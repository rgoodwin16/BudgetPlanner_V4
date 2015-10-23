(function () {
    angular.module('budget_planner')
    .factory('categorySvc', ['$http', function ($http) {
        var f = {};

        f.list = function () {
            return $http.post('api/Household/Categories/Index').then(function (response) {
                return response.data
            })
        }

        return f;

    }])
})();