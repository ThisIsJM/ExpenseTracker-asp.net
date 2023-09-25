let incomeChart
let expensesChart
let balanceChart
let allTransactions = [];

// _+_+_+_+_+_+_+_+_+_+ GENERATE CHARTS _+_+_+_+_+_+_+_+_+_+_+_+_+_
async function generateTransactionCharts() {

    const dateValues = allTransactions.map(t => new Date(t.date));

    // Find the earliest date using Math.min.apply
    const earliestDate = new Date(Math.min.apply(null, dateValues));

    try {
        const response = await fetch("/Home/GetAllTransactions");

        if (!response.ok) {
            throw new Error(`Request failed with status ${response.status}`);
        }

        const transactions = await response.json();

        const { incomeSummary, expensesSummary } = getTransactionsBreakdownSummary(transactions);
        const balanceGrowth = getBalanceGrowth(allTransactions, earliestDate)
        generateCharts(incomeSummary, expensesSummary, balanceGrowth);
    } catch (error) {
        generateCharts([], [], []);
    }
}

async function generateThisMonthTransactionCharts() {

    let now = new Date()
    let lastMonth = new Date(now.getFullYear(), now.getMonth(), now.getDate() - 30)

    try {
        const response = await fetch("/Home/GetThisMonthTransactions");

        if (!response.ok) {
            throw new Error(`Request failed with status ${response.status}`);
        }

        const transactions = await response.json();
        console.log(transactions);

        const { incomeSummary, expensesSummary } = getTransactionsBreakdownSummary(transactions);
        const balanceGrowth = getBalanceGrowth(allTransactions, lastMonth);

        generateCharts(incomeSummary, expensesSummary, balanceGrowth);
    } catch (error) {
        generateCharts([], [], []);
    }
}

async function generateThisWeekTransactionCharts() {

    let now = new Date()
    let lastWeek = new Date(now.getFullYear(), now.getMonth(), now.getDate() - 7)

    try {
        const response = await fetch("/Home/GetThisWeekTransactions");

        if (!response.ok) {
            throw new Error(`Request failed with status ${response.status}`);
        }

        const transactions = await response.json();
        console.log(transactions);

        const { incomeSummary, expensesSummary } = getTransactionsBreakdownSummary(transactions);
        const balanceGrowth = getBalanceGrowth(allTransactions, lastWeek);

        generateCharts(incomeSummary, expensesSummary, balanceGrowth);
    } catch (error) {
        generateCharts([], [], []);
    }
}

function getTransactionsBreakdownSummary(transactions) {

    const incomeSummary = { descriptions: [], totalAmount: [] }
    const expensesSummary = { descriptions: [], totalAmount: [] }

    transactions.forEach(transaction => {

        if (transaction.type == 0) {

            const index = incomeSummary.descriptions.indexOf(transaction.description);

            if (index === -1) {
                incomeSummary.descriptions.push(transaction.description);
                incomeSummary.totalAmount.push(transaction.amount);
            }
            else {
                incomeSummary.totalAmount[index] += transaction.amount;
            }
        }
        else {
            const index = expensesSummary.descriptions.indexOf(transaction.description);

            if (index === -1) {
                expensesSummary.descriptions.push(transaction.description);
                expensesSummary.totalAmount.push(transaction.amount);
            }
            else {
                expensesSummary.totalAmount[index] += transaction.amount;
            }
        }
    })

    return {incomeSummary, expensesSummary}
}

function getThisWeekBalanceGrowth() {

    //GET LAST WEEK DATE
    let now = new Date()
    let lastWeek = new Date(now.getFullYear(), now.getMonth(), now.getDate() - 7,)

    //GET INITIAL BALANCE
    let initialBalance = 0;
    allTransactions.map(t => { if (new Date(t.date) <= lastWeek) initialBalance += t.type === 0 ? t.amount : -t.amount;})

    //COMPUTE DAILY BALANCE GROWTH FROM LAST WEEK TO NOW
    let weeklyTransactions = allTransactions.filter(t => new Date(t.date) >= lastWeek && new Date(t.date) <= now)
    let balanceGrowth = { dates: [], totalBalance: [] }
    let currentDate = new Date(lastWeek)

    while (currentDate <= now) {

        // Filter transactions for the current date
        let transactionsForDate = weeklyTransactions.filter(t => 
            new Date(t.date).toDateString() === currentDate.toDateString()
        );

        // Calculate the total balance for the current date
        let dailyTotalBalance = transactionsForDate.reduce((balance, t) => {
            return balance + (t.type === 0 ? t.amount : -t.amount);
        }, initialBalance);

        // Store the date and total balance
        balanceGrowth.dates.push(currentDate.toDateString());
        balanceGrowth.totalBalance.push(dailyTotalBalance);

        // Move to the next day
        currentDate.setDate(currentDate.getDate() + 1);
    }

    return balanceGrowth;
}

function getBalanceGrowth(transactions, startDate) {

    let now = new Date();

    //GET INITIAL BALANCE
    let initialBalance = 0;
    transactions.map(t => { if (new Date(t.date) <= startDate) initialBalance += t.type === 0 ? t.amount : -t.amount; })

    //COMPUTE DAILY BALANCE GROWTH FROM LAST WEEK TO NOW
    let filteredTransactions = transactions.filter(t => new Date(t.date) >= startDate && new Date(t.date) <= now)
    let balanceGrowth = { dates: [], totalBalance: [] }
    let currentDate = new Date(startDate)

    while (currentDate <= now) {

        // Filter transactions for the current date
        let transactionsForDate = filteredTransactions.filter(t =>
            new Date(t.date).toDateString() === currentDate.toDateString()
        );

        // Calculate the total balance for the current date
        let dailyTotalBalance = transactionsForDate.reduce((balance, t) => {
            return balance + (t.type === 0 ? t.amount : -t.amount);
        }, initialBalance);

        // Store the date and total balance
        balanceGrowth.dates.push(currentDate.toDateString());
        balanceGrowth.totalBalance.push(dailyTotalBalance);

        // Move to the next day
        currentDate.setDate(currentDate.getDate() + 1);
    }

    return balanceGrowth;
}


// _+_+_+_+_+_+_+_+_ FUNCTION GENERATE CHARTS _+_+_+_+_+_+_+_

function generateCharts(incomeSummary, expensesSummary, balanceGrowth) {

    const balanceChartCanvas = document.getElementById('balanceChartCanvas');
    const incomeChartCanvas = document.getElementById('incomeChartCanvas')
    const expensesChartCanvas = document.getElementById('expensesChartCanvas')

    // Destroy existing charts if they exist
    if (incomeChart) incomeChart.destroy();

    if (expensesChart) expensesChart.destroy();

    if (balanceChart) balanceChart.destroy();

    //INCOME CHART
    incomeChart = new Chart(incomeChartCanvas, {
        type: 'pie',
        data: {
            labels: incomeSummary.descriptions,
            datasets: [{
                data: incomeSummary.totalAmount,
                hoverOffset: 4
            }]
        },
    });

    //EXPENSES CHART
    expensesChart = new Chart(expensesChartCanvas, {
        type: 'pie',
        data: {
            labels: expensesSummary.descriptions,
            datasets: [{
                data: expensesSummary.totalAmount,
                hoverOffset: 4
            }]
        },
    });

    //BALANCE CHART
    balanceChart = new Chart(balanceChartCanvas, {
        type: 'line',
        data: {
            labels: balanceGrowth.dates,
            datasets: [{
                label: 'Total Balance Growth',
                data: balanceGrowth.totalBalance,
                fill: false,
                borderColor: 'rgb(75, 192, 192)',
                tension: 0.1
            }]
        },
        options: {
            scales: {
                x: {
                    display: false
                }
            }
        }
    });

}


// _+_+_+_+_+_+_+_+_+_+ INITIAL ANALYSIS LOAD _+_+_+_+_+_+_+_+_
document.addEventListener('DOMContentLoaded', async function () {

    try {
        const response = await fetch("/Home/GetAllTransactions");

        if (!response.ok) {
            throw new Error(`Request failed with status ${response.status}`);
        }

        allTransactions = await response.json();
        console.log(allTransactions);

    } catch (error) {
        console.log(error)
    }

    getThisWeekBalanceGrowth();
    generateTransactionCharts();

})
