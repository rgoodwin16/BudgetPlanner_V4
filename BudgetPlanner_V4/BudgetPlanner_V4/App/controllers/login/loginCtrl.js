'use strict';
angular.module('budget_planner').controller('loginCtrl', ['authSvc', '$state', '$stateParams','spinnerService',  function (authSvc, $state, $stateParams,spinnerService) {
    var self = this;

    self.username = '';
    self.password = '';
    self.isNew = $stateParams.isNew === true;
    self.errors = null;

    //LOGIN FORM SUBMIT - EXISTING USER
    self.login = function () {
        spinnerService.show('defaultSpinner');
        authSvc.login(self.username, self.password).then(function (success) {
            spinnerService.hide('defaultSpinner');
            $state.go('dashboard');
        }, function (error) {
            spinnerService.hide('defaultSpinner');
            self.errors = error;
        });
    }

}])