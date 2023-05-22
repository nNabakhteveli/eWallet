const urlParams = new URLSearchParams(window.location.search);
const amount = urlParams.get('amount');
const transactionId = urlParams.get('transactionId');


$(document).ready(() => {
    $('.transaction-id-text').text(`Transaction ID: ${transactionId}`);
    $('.main-text').text(`Accept or reject your payment of $${amount} here.`);

    if (!transactionId || !amount) {
        alert('INVALID Request!');
        
    } else {
        $('.accept-btn').on('click', () => {
            $.ajax({
                type: 'POST',
                url: 'https://localhost:7039/Transactions/Api/AcceptDeposit',
                data: {success: true, transactionId: transactionId},
                success: data => {
                    const success = data.success === true ? true : false;
                    window.location.href = `https://localhost:7039/Identity/Account/Wallet?success=${success}&amount=${amount}`;
                }
            });
        });
    }


    $('.reject-btn').on('click', () => {
        $.ajax({
            type: 'POST',
            url: 'https://localhost:7039/Transactions/Api/RejectDeposit',
            data: {success: false, transactionId: transactionId},
            complete: data => {
                window.location.href = "https://localhost:7039/Identity/Account/Wallet?success=false";
            }
        });
    });
});

console.log(amount, transactionId);
