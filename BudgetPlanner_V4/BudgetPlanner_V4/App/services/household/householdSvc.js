(function () {
    angular.module('budget_planner')
    .factory('householdSvc', ['$http', function ($http) {
        var f = {};

        f.details = function () {
            return $http.post('api/Household/Details').then(function (response) {
                return response.data
            })
        }

        f.create = function (name) {
            return $http.post('api/Household/Create?name=' + name).then(function (response) {
                return response.data
            })
        }

        f.invite = function (email) {
            return $http.post('api/Household/Invite?inviteEmail=' + email).then(function (response) {
                return response.data
            })
        }

        f.join = function (model) {
            return $http.post('api/Household/Join', model).then(function (response) {
                return response.data
            })
        }

        f.leave = function () {
            return $http.post('api/Household/Leave').then(function (response) {
                return response.data
            })
        }

        return f;
    }])
})();