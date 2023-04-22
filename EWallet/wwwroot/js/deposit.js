function setPublicToken(token) {
    localStorage.setItem('publicToken', token);
}

$(document).ready(() => {
    $('.token-generator-btn').on('click', () => {
        $('.public-token').text(`Public Token: ${localStorage.getItem('publicToken')}`);
    });
    
    $(".deposit-btn").on("click", (event) => {
        event.preventDefault();

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

        let currencySign = "$";
        const transaction = {
            UserId: $(".UserId").val(),
            RecipientId: $(".RecipientId").val(),
            PaymentType: "Deposit",
            Amount: $(".Amount").val(),
            Currency: $(".Currency").val(),
            Status: 0
        }

        if (transaction.Currency === "GEL") currencySign = "₾";
        else if (transaction.Currency === "EUR") currencySign = "€";

        $.ajax({
            type: 'POST',
            url: '/transactions/api/deposit',
            data: transaction,
            success: data => {
                if (data.success) {
                    successToast.fire({
                        icon: 'success',
                        title: `Success! ${currencySign}${transaction.Amount} sent to the recipient. The deposit is in the pending stage.`
                    });
                } else {
                    errorToast.fire({
                        icon: 'error',
                        title: 'Failure'
                    });
                }
            }
        });
    });
});