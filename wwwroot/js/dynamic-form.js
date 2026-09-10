$(document).ready(function () {

    var $form = $(".dynamic-form");

    /**
     * Re-indexes form attributes (name, data-valmsg-for) for dynamically added/removed rows.
     * Ensures ASP.NET Core model binding and validation work correctly.
     */
    function reindexRows() {
        $('#items-tbody .item-row').each(function (index) {

            // Update input and select names
            $(this).find('input, select').each(function () {
                let name = $(this).attr('name');
                if (name) {
                    $(this).attr('name', name.replace(/\[\d+\]/, `[${index}]`));
                }
            });

            // Update validation span attributes
            $(this).find('.field-validation-valid, .field-validation-error').each(function () {
                let valmsg = $(this).attr('data-valmsg-for');
                if (valmsg) {
                    $(this).attr('data-valmsg-for', valmsg.replace(/\[\d+\]/, `[${index}]`));
                }
            });
        });

        // Re-parse the form for unobtrusive validation
        $form.removeData('validator');
        $form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse($form);
    }

    /**
     * Calculates line totals for all rows and updates the grand total.
     */
    function calculateTotals() {
        let grandTotal = 0;

        $('#items-tbody .item-row').each(function () {
            let qty = parseFloat($(this).find('.qty-input').val()) || 0;
            let cost = parseFloat($(this).find('.cost-input').val()) || 0;

            let lineTotal = qty * cost;
            grandTotal += lineTotal;

            $(this).find('.line-total').text('$' + lineTotal.toFixed(2));
        });

        $('#grand-total').text('$' + grandTotal.toFixed(2));
    }

    /**
     * Prevents selecting the same product multiple times in different rows.
     */
    function checkDuplicates(selectElem) {
        let currentVal = $(selectElem).val();
        if (!currentVal) return;

        let count = 0;
        $('#items-tbody .product-select').each(function () {
            if ($(this).val() === currentVal) count++;
        });

        if (count > 1) {
            alert('This item is already selected. Please adjust the quantity of the existing row instead.');
            $(selectElem).val('');
        }
    }


    // ================= Event Listeners =================

    // 1. Handle product selection (Works for both Sales and Purchases)
    $('#items-tbody').on('change', '.product-select', function () {
        checkDuplicates(this);

        let selectedOption = $(this).find('option:selected');
        let row = $(this).closest('tr');
        let qtyInput = row.find('.qty-input');
        let costInput = row.find('.cost-input');

        // Populate unit price if available (Sales context)
        let price = selectedOption.attr('data-price');
        if (price !== undefined) {
            costInput.val(price);
        }

        // Handle stock validation if available (Sales context)
        let stock = selectedOption.attr('data-stock');
        if (stock !== undefined && stock !== "") {
            stock = parseInt(stock, 10);
            qtyInput.attr('max', stock);

            // Prepare inline validation hint (hidden by default)
            let stockHint = row.find('.stock-hint');
            if (stockHint.length === 0) {
                qtyInput.after('<small class="text-danger stock-hint mt-1 fw-bold" style="display: none;"></small>');
            } else {
                stockHint.hide();
            }

            // Adjust quantity silently if the current input exceeds the new product's stock
            if (parseInt(qtyInput.val(), 10) > stock) {
                qtyInput.val(stock);
                row.find('.stock-hint').text(`Max available stock is ${stock}`).show();
            }
        } else {
            // Purchases context: clear max constraint and hide hint
            qtyInput.removeAttr('max');
            row.find('.stock-hint').hide();
        }

        calculateTotals();
    });

    // 2. Handle quantity changes (Enforces max stock limit dynamically)
    $('#items-tbody').on('input', '.qty-input', function () {
        let maxStr = $(this).attr('max');
        let row = $(this).closest('tr');
        let stockHint = row.find('.stock-hint');

        if (maxStr !== undefined) {
            let max = parseInt(maxStr, 10);
            let val = parseInt($(this).val(), 10) || 0;

            // Validate against max stock
            if (val > max) {
                stockHint.text(`Max available stock is ${max}`).show();
                $(this).val(max);
            } else {
                stockHint.hide();
            }
        }

        calculateTotals();
    });

    // 3. Handle manual cost input changes (Purchases context)
    $('#items-tbody').on('input', '.cost-input', calculateTotals);

    // 4. Remove an item row
    $('#items-tbody').on('click', '.btn-remove', function () {
        if ($('#items-tbody .item-row').length > 1) {
            $(this).closest('tr').remove();
            reindexRows();
            calculateTotals();
        } else {
            alert('The document must contain at least one item.');
        }
    });

    // 5. Add a new item row
    $('#btn-add-item').click(function () {
        let newRow = $('#row-template').html();
        $('#items-tbody').append(newRow);

        // Re-initialize Lucide icons for the new row
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }

        reindexRows();
        calculateTotals();
    });

    // Initialization
    calculateTotals();
});