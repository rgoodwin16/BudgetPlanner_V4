(function () {
    angular.module('budget_planner')
    .factory('householdAccountSvc', ['$http', function ($http) {
        var f = {};

        f.list = function () {
            return $http.post('api/HouseholdAccounts/Index').then(function (response) {
                return response.data
            })
        }

        f.create = function (account) {
            return $http.post('api/HouseholdAccounts/Create', account).then(function (response) {
                return response.data
            })
        }

        f.details = function (id) {
            return $http.post('api/HouseholdAccounts/Details?id=' + id).then(function (response) {
                return response.data
            })
        }

        f.edit = function (account) {
            return $http.post('api/HouseholdAccounts/Edit', account).then(function (response) {
                return response.data
            })
        }

        f.archive = function (id) {
            return $http.post('api/HouseholdAccounts/Archive?id=' + id).then(function (response) {
                return response.data
            })
        }

        f.reclaim = function (id) {
            return $http.post('api/HouseholdAccounts/Reclaim?id=' + id).then(function (response) {
                return response.data
            })
        }

        return f;
    }])
})();