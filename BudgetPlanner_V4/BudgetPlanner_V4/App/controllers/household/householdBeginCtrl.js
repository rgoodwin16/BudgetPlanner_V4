'use strict';
angular.module('budget_planner').controller('householdBeginCtrl', ['authSvc', '$state', 'householdSvc', function (authSvc, $state, householdSvc) {
    var self = this;
    self.$state = $state;

    this.name = '';
    this.model = {};
    this.createErrors = null;
    this.joinErrors = null;
    

    //CREATE HOUSEHOLD
    this.create = function () {
        householdSvc.create(self.name).then(function (success) {
            authSvc.refresh().then(function (response) {
                $state.go($state.current, null, { reload: true })
                $state.go('household.details');
            })
        }, function (error) {
            self.createErrors = error;
        });
    }

    //JOIN HOUSEHOLD
    this.join = function () {
        householdSvc.join(self.model).then(function (success) {
            authSvc.refresh().then(function (response) {
                $state.go($state.current, null, { reload: true })
                $state.go('household.details');
            })
        }, function (error) {
            self.joinErrors = error;
        });
    }

}])