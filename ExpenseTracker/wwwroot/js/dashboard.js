

//CLEAR TRANSACTION MODAL
function resetTransactionModal() {
    //UPDATE MODEL VALUES AND ALSO OPEN A MODAL
    $("#transactionModal #TransactionId").val(0);
    $("#transactionModal #TransactionType").val('');
    $("#transactionModal #Description").val('');
    $("#transactionModal #Amount").val('');
    $("#transactionModal #Date").val(new Date().toISOString().split('T')[0]);

    //CHANGE FORM ASP-ACTION TO EDIT
    $("#transactionForm").attr("action", "/Home/Create");
}

//UPDATE
function editTransaction(transactionId) {
    $.ajax({
        type: "GET",
        url: "/Home/GetTransactionAsync",
        data: { id: transactionId },
        success: function (data) {
            console.log(data)

            // Adjust for local time zone offset
            var date = new Date(data.date);
            var localDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
            var formattedDate = localDate.toISOString().split('T')[0];

            //UPDATE MODEL VALUES AND ALSO OPEN A MODAL
            $("#transactionModal #TransactionId").val(data.id);
            $("#transactionModal #TransactionType").val(data.type);
            $("#transactionModal #Description").val(data.description);
            $("#transactionModal #Amount").val(data.amount);
            $("#transactionModal #Date").val(formattedDate);

            //OPEN MODAL
            $("#transactionModal").modal("show");

            //CHANGE FORM ASP-ACTION TO EDIT
            $("#transactionForm").attr("action", "/Home/Edit");
        },
        error: function () {
            console.log(transactionId)
            // Handle error (e.g., show an error message)
        }
    });
}

//DELETE
function deleteTransaction(transactionId) {
    // Send a POST request to the Delete action with the transaction ID
    $.ajax({
        type: "POST",
        url: "/Home/Delete",
        data: { id: transactionId },
        success: function () {
            location.reload();
        },
        error: function () {
            // Handle error (e.g., show an error message)
        }
    });
}

function filterDate(dateFilter) {

    var transactionTable = document.getElementById("TransactionTable");
    var rows = transactionTable.getElementsByTagName("tr")

    for (var i = 0; i < rows.length; i++) {
        var cell = rows[i].getElementsByTagName("td")[3];

        if (cell) {
            var cellDate = new Date(cell.textContent);
            var isVisible = isWithinDateRange(cellDate, dateFilter);

            rows[i].style.display = isVisible ? "" : "none";
        }
    }

    var filteredAmounts = computeFilteredAmounts(dateFilter, rows);

    //GET DOM ELEMENTS
    var income = document.getElementById("Income Card")
    var expenses = document.getElementById("Expenses Card")
    var balance = document.getElementById("Balance Card")

    //CHANGE VALUES
    income.querySelector("h1").textContent = filteredAmounts.totalIncome;
    expenses.querySelector("h1").textContent = filteredAmounts.totalExpenses;
    balance.querySelector("h1").textContent = filteredAmounts.totalBalance;
}

function computeFilteredAmounts(dateFilter, rows) {

    let totalIncome = 0;
    let totalExpenses = 0;

    for (var i = 0; i < rows.length; i++) {

        var transactionTypeCell = rows[i].getElementsByTagName("td")[0];
        var transactionAmountCell = rows[i].getElementsByTagName("td")[2];
        var transactionDateCell = rows[i].getElementsByTagName("td")[3]

        if (!transactionTypeCell || !transactionAmountCell || !transactionDateCell) continue;

        // Parse the content of the cells to numbers
        var transactionType = transactionTypeCell.textContent;
        var transactionAmount = parseFloat(transactionAmountCell.textContent);
        var transactionDate = new Date(transactionDateCell.textContent)

        if (!isWithinDateRange(transactionDate, dateFilter)) continue;

        //INCOME
        if (transactionType == "Income") {
            totalIncome += transactionAmount;
        }
        else {
            totalExpenses += transactionAmount;
        }
    }

    let totalBalance = totalIncome - totalExpenses
    return {
        totalIncome,
        totalExpenses,
        totalBalance
    }
}

function isWithinDateRange(transactionDate, dateFilter) {

    let currentDate = new Date()

    switch (dateFilter) {
        case "All":
            return true;

        case "This Month":
            return currentDate - transactionDate <= 30 * 24 * 60 * 60 * 1000

        case "This Week":
            return currentDate - transactionDate <= 7 * 24 * 60 * 60 * 1000

        case "Today":
            return currentDate.toDateString() === transactionDate.toDateString()

        default:
            return false;
    }
}





//DROPDOWN TOGGLE
/*$('#dropdownIcon').click(function () {
    $('.dropdown-menu').toggle();
});*/