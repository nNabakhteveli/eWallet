const urlParams = new URLSearchParams(window.location.search);
const amount = urlParams.get('amount');
const transactionId = urlParams.get('transactionId');


$(document).ready(() => {
    $('.transaction-id-text').text(`Transaction ID: ${transactionId}`);
    $('.main-text').text(`Accept or reject your payment of $${amount} here.`);

    $('.accept-btn').on('click', () => {
        $.ajax({
            type: 'POST',
            url: 'https://localhost:7039/Transactions/Api/AcceptDeposit',
            data: { success: true, transactionId: transactionId},
            success: data => {
                window.location.href = `https://localhost:7039/Identity/Account/Wallet?success=true&amount=${amount}`;
            }
        });
    });
    
    
    $('.reject-btn').on('click', () => {
        $.ajax({
            type: 'POST',
            url: 'https://localhost:7039/Transactions/Api/RejectDeposit',
            data: { success: false, transactionId: transactionId},
            success: data => {
                window.location.href = "https://localhost:7039/Identity/Account/Wallet?success=false";
            }
        });
    });
});

console.log(amount, transactionId);
