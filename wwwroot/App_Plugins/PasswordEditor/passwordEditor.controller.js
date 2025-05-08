(function () {
    'use strict';

    function passwordEditorController($scope) {
        var vm = this;

        // Initialize properties
        vm.showPassword = false;
        vm.passwordStrength = "none";

        // Toggle password visibility
        vm.togglePasswordVisibility = function () {
            vm.showPassword = !vm.showPassword;
        };

        // Calculate password strength
        function calculatePasswordStrength(password) {
            if (!password) return "none";

            var strength = 0;

            // Length check
            if (password.length >= 8) strength++;

            // Contains uppercase
            if (/[A-Z]/.test(password)) strength++;

            // Contains lowercase
            if (/[a-z]/.test(password)) strength++;

            // Contains numbers
            if (/[0-9]/.test(password)) strength++;

            // Contains special characters
            if (/[^A-Za-z0-9]/.test(password)) strength++;

            // Return strength level
            switch (strength) {
                case 0: case 1: return "weak";
                case 2: case 3: return "medium";
                case 4: case 5: return "strong";
                default: return "none";
            }
        }

        // Watch for changes to update password strength
        $scope.$watch('model.value', function (newValue) {
            vm.passwordStrength = calculatePasswordStrength(newValue);
        });
    }

    angular.module("umbraco").controller("My.PasswordEditorController", passwordEditorController);
}());