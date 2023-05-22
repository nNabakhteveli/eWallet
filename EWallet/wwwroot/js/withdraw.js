const urlParams = new URLSearchParams(window.location.search);
const success = urlParams.get('success');
const amount = urlParams.get('amount');


$(document).ready(() => {
    const successToast = Swal.mixin({
        toast: true,
        position: 'top-right',
        iconColor: 'green',
        customClass: {
            popup: 'colored-toast'
        },
        showConfirmButton: false,
        timer: 4000,
        timerProgressBar: true
    });

    const errorToast = Swal.mixin({
        toast: true,
        position: 'top-right',
        iconColor: 'red',
        customClass: {
            popup: 'colored-toast'
        },
        showConfirmButton: false,
        timer: 2500,
        timerProgressBar: true
    });

    if (success === 'true') {
        successToast.fire({
            icon: 'success',
            title: `Success! The Withdraw of $${amount} has finished successfuly.`
        });
    } else if (success === 'false'){
        errorToast.fire({
            icon: 'error',
            title: 'Failure'
        });
    }

    $(".withdraw-btn").on("click", (event) => {
        event.preventDefault();
        
        const transaction = {
            UserId: $(".UserId").val(),
            PaymentType: "Withdraw",
            Amount: $(".Amount").val(),
            Currency: $(".Currency").val(),
            Status: 0
        }
        
        $.ajax({
            type: 'POST',
            url: '/transactions/api/withdraw',
            data: transaction,
            success: data => {
                if (data.transactionId === -1) {
                    alert("INVALID OPERATION")
                } else {
                    window.location.href = `https://localhost:7106/?amount=${data.amount}&transactionId=${data.transactionId}&type=withdraw`;
                }
            }
        });
    });
});