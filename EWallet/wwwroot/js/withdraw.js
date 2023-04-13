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

    $(".withdraw-btn").on("click", (event) => {
        event.preventDefault();

        let currencySign = "$";
        const transaction = {
            UserId: $(".UserId").val(),
            PaymentType: "Withdraw",
            Amount: $(".Amount").val(),
            Currency: $(".Currency").val(),
            Status: 0
        }

        if (transaction.Currency === "GEL") currencySign = "₾";
        else if (transaction.Currency === "EUR") currencySign = "€";

        $.ajax({
            type: 'POST',
            url: '/transactions/api/withdraw',
            data: transaction,
            success: data => {
                if (data.success) {
                    successToast.fire({
                        icon: 'success',
                        title: `Success! The Withdraw is in the pending stage now.`
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