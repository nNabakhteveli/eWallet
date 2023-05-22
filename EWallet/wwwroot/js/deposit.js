const urlParams = new URLSearchParams(window.location.search);
const success = urlParams.get('success');
const amount = urlParams.get('amount');

function setPublicToken(userId) {
    $.ajax({
        type: 'POST',
        url: '/token/generate',
        contentType: "application/json",
        data: JSON.stringify({UserId: userId}),
        success: data => {
            if (data.success) {
                $('.public-token').text(`Public Token: ${data.publicToken}`);
                $('.token-generator-btn').remove();
            } else {
                alert("Something went wrong!..");
            }
        }
    });
}

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
    timer: 1500,
    timerProgressBar: true
});

$(document).ready(() => {
    if (success === 'false') {
        errorToast.fire({
            icon: 'error',
            title: 'Failure'
        });
    }

    if (success === 'true') {
        successToast.fire({
            icon: 'success',
            title: `Success! $${amount} sent to the recipient. The deposit finished successfuly`
        });
    }

    $(".deposit-btn").on("click", (event) => {
        event.preventDefault();
        
        const transaction = {
            UserId: $(".UserId").val(),
            PaymentType: "Deposit",
            Amount: $(".Amount").val(),
            Currency: $(".Currency").val(),
            Status: 0
        }

        $.ajax({
            type: 'POST',
            url: '/transactions/api/deposit',
            data: transaction,
            success: data => {
                window.location.href = `https://localhost:7106/?amount=${data.amount}&transactionId=${data.transactionId}&type=deposit`;
            }
        });
    });
});