'use strict';
angular.module('budget_planner').controller('dashboardCtrl', ['$state', '$http', 'dashboardSvc', 'currentMonth', 'currentValues', 'yearlyValues', 'account', 'transactions', function ($state, $http, dashboardSvc, currentMonth, currentValues, yearlyValues, account, transactions) {

    var self = this;

    this.dates = currentMonth;
    this.account = account;
    this.transactions = transactions;
    this.currentpanel = "c";

    self.$state = $state;

    self.currentValues = currentValues;
    self.yearlyValues = yearlyValues;

    self.selectedValues = [];
    self.model = {};

    self.currentOptions = {
        chart: {
            type: 'multiBarChart',
            height: 450,
            transitionDuration: 500
        }
    }

    self.selectedOptions = {
        chart: {
            type: 'multiBarChart',
            height: 450,
            transitionDuration: 500
        }
    }

    self.yearlyOptions = {
        chart: {
            type: 'multiBarChart',
            height: 450,
            transitionDuration: 500
        }
    }

    this.clear = function () {
        $state.go($state.current, null, { reload: true })
    }

    this.selectDates = function () {
        self.currentpanel = "u";
    }

    this.createChart = function () {
        dashboardSvc.sValues(self.model).then(function (result) {
            self.selectedValues = result;
        })
    }

}])