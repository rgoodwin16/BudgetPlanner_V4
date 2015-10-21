(function () {
    angular.module('budget_planner')
    .factory('dashboardSvc', ['$http', function ($http) {
        var f = {};

        f.dates = function () {
            return $http.post('api/Dashboard/Dates').then(function (response) {
                return response.data
            })
        }

        f.cValues = function () {
            return $http.post('/api/Dashboard/Current').then(function (response) {
                return response.data
            })
        }

        f.sValues = function (model) {
            return $http.post('api/Dashboard/Selected', model).then(function (response) {
                return response.data
            })
        }

        f.yValues = function () {
            return $http.post('api/Dashboard/Yearly').then(function (response) {
                return response.data
            })
        }

        return f;
    }])
})();