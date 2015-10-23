(function () {
    angular.module('budget_planner').factory('validationSvc', function () {
        return {
            isEmail: function (email) {
                var re = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
                return re.test(email);
            }
        }
    })
})();