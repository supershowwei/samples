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
            
            // $http handle HTTP and Service result fully
            self.save = function () {
                console.log(self.application);
                $http.post("/contenteditabletable/new", self.application)
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
                        function unsuccess(data) {
                            console.log(data);
                            if (data.isFailure) {
                                return $q.resolve(response.data);
                            } else {
                                return $q.reject(response.data);
                            }
                        })
                    .then(function failure(data) {
                            console.error(data);
                        },
                        function warning(data) {
                            console.warn(data);
                        });
            };
        }
    ]);