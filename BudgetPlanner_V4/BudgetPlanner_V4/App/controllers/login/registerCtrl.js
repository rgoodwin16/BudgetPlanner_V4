'use  strict';
angular.module('budget_planner').controller('registerCtrl', ['authSvc', '$state', function (authSvc, $state) {
    var self = this;

    this.toggle = 0;
    this.model = {};

    self.errors = null;
    

    //SIGNUP
    this.signup = function () {
        authSvc.register(self.model).then(function (success) {
            $state.go('login.signin',{ isNew:true })
        }, function (error){
            self.errors = error;
            console.log(self.errors)
        })
    }

}])