var Receipt = function () {
    this.summary = "";
    this.date = "";
    this.number = "";
    this.noTaxAmount = 0;
    this.tax = function () {
        return Number(this.noTaxAmount) * 0.05;
    };
    this.amount = function () {
        return Number(this.noTaxAmount) + Number(this.tax());
    };
    this.memo = "";
};