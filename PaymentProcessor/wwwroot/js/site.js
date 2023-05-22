const urlParams = new URLSearchParams(window.location.search);
const amount = urlParams.get('amount');
const transactionId = urlParams.get('transactionId');
const transactionType = urlParams.get('type');

const deposit = 'deposit';
const withdraw = 'withdraw';

$(document).ready(() => {
    $('.transaction-id-text').text(`Transaction ID: ${transactionId}`);
    if (transactionType === 'deposit') {
        $('.main-text').text(`Accept or reject your payment of $${amount} here.`);
    } else {
        $('.main-text').text(`Accept or reject withdraw of $${amount} here.`);
    }

    if (!transactionId || !amount) {
        alert('INVALID Request!');

    } else {

        $('.accept-btn').on('click', () => {
            if (transactionType === deposit) {
                $.ajax({
                    type: 'POST',
                    url: 'https://localhost:7039/Transactions/Api/AcceptDeposit',
                    data: {success: true, transactionId: transactionId},
                    success: data => {
                        window.location.href = `https://localhost:7039/Identity/Account/Wallet?success=${data.success}&amount=${amount}`;
                    }
                });
            } else if (transactionType === withdraw) {
                $.ajax({
                    type: 'POST',
                    url: 'https://localhost:7039/Transactions/Api/AcceptWithdraw',
                    data: {transactionId: transactionId},
                    success: data => {
                        window.location.href = `https://localhost:7039/Identity/Account/Withdraw?success=${data.success}&amount=${amount}`;
                    }
                });
            }
        });

    }


    $('.reject-btn').on('click', () => {
        if (transactionType === deposit) {
            $.ajax({
                type: 'POST',
                url: 'https://localhost:7039/Transactions/Api/RejectDeposit',
                data: {success: false, transactionId: transactionId},
                complete: data => {
                    window.location.href = "https://localhost:7039/Identity/Account/Wallet?success=false";
                }
            });
        } else if (transactionType === withdraw) {
            $.ajax({
                type: 'POST',
                url: 'https://localhost:7039/Transactions/Api/RejectWithdraw',
                data: {transactionId: transactionId},
                complete: data => {
                    window.location.href = `https://localhost:7039/Identity/Account/Withdraw?success=false`;
                }
            });
        }
    });
});

console.log(amount, transactionId);
