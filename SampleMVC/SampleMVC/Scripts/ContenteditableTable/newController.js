angular.module("newApp")
    .controller("newController",
    [
        "$http", "$q", "$window",
        function ($http, $q, $window) {
            var self = this;

            self.receipts = [];

            self.addReceipt = function () {
                self.receipts.push(new Receipt());
            };

            self.save = function () {
                console.log(self.application);
                $http.post("/application/new", self.application)
                    .then(function ok(response) {
                            console.log(response);
                            if (response.data.isSuccess) {
                                return $q.resolve(response.data);
                            } else {
                                return $q.reject(response.data);
                            }
                        },
                        function error(ex) {
                            console.error(ex);
                        })
                    .then(function success(data) {
                            console.log(data);
                        },
                        function failure(data) {
                            console.error(data);
                        });
            };
        }
    ]);