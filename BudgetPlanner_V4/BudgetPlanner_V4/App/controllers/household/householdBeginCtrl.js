'use strict';
angular.module('budget_planner').controller('householdBeginCtrl', ['authSvc', '$state', 'householdSvc', function (authSvc, $state, householdSvc) {
    var self = this;
    self.$state = $state;

    this.name = '';
    this.model = {};

    //CREATE HOUSEHOLD
    this.create = function () {
        householdSvc.create(self.name).then(function (result) {
            authSvc.refresh().then(function (response) {
                $state.go($state.current, null, { reload: true })
                $state.go('budget.list');
            })
        })
    }

    //JOIN HOUSEHOLD
    this.join = function () {
        householdSvc.join(self.model).then(function (result) {
            authSvc.refresh().then(function (response) {
                $state.go($state.current, null, { reload: true })
                $state.go('household.details');
            })
        })
    }

}])