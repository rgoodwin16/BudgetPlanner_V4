'use strict';
angular.module('budget_planner').controller('loginCtrl', ['authSvc', '$state', '$stateParams', function (authSvc, $state, $stateParams) {
    var self = this;

    self.username = '';
    self.password = '';
    self.isNew = $stateParams.isNew === true;
    self.errors = null;

    //LOGIN FORM SUBMIT - EXISTING USER
    self.login = function () {
        authSvc.login(self.username, self.password).then(function (success) {
            $state.go('household.details');
        }, function (error) {
            self.errors = error;
        });
    }

}])