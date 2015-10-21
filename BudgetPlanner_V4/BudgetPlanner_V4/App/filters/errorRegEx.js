
app.filter('removeBrackets', function () {
    return function (item) {
        return item.replace(/[\[\]']+/g,'');
    }
});