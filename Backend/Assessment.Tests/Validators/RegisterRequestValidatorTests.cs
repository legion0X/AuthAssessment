using Assessment.Application.DTOs;
using Assessment.Application.Validators;
using FluentValidation.TestHelper;

namespace Assessment.Tests.Validators
{
    public class RegisterRequestValidatorTests
    {
        private readonly RegisterRequestValidator _validator;

        public RegisterRequestValidatorTests()
        {
            _validator = new RegisterRequestValidator();
        }

        [Fact]
        public void ShouldNotHaveValidationErrors_WhenValidModelIsProvided()
        {
            var model = new RegisterRequestDTO("test@example.com", "StrongPass1!");
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void ShouldHaveValidationError_WhenEmailIsEmpty()
        {
            var model = new RegisterRequestDTO("", "ValidPass1!");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Email is required.");
        }

        [Fact]
        public void ShouldHaveValidationError_WhenEmailIsInvalid()
        {
            var model = new RegisterRequestDTO("invalid-email", "ValidPass1!");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Invalid email format.");
        }

        [Fact]
        public void ShouldHaveValidationError_WhenEmailContainsSpaces()
        {
            var model = new RegisterRequestDTO("test @email.com", "ValidPass1!");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Email cannot contain spaces.");
        }

        [Fact]
        public void ShouldHaveValidationError_WhenEmailExceedsMaxLength()
        {
            var longEmail = new string('a', 101) + "@example.com";
            var model = new RegisterRequestDTO(longEmail, "ValidPass1!");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Email must be at most 100 characters.");
        }

        [Fact]
        public void ShouldHaveValidationError_WhenPasswordIsEmpty()
        {
            var model = new RegisterRequestDTO("test@example.com", "");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password is required.");
        }

        [Fact]
        public void ShouldHaveValidationError_WhenPasswordIsTooShort()
        {
            var model = new RegisterRequestDTO("test@example.com", "Short1!");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password must be at least 8 characters.");
        }

        [Fact]
        public void ShouldHaveValidationError_WhenPasswordExceedsMaxLength()
        {
            var longPassword = new string('a', 51);
            var model = new RegisterRequestDTO("test@example.com", longPassword);
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password must be at most 50 characters.");
        }

        [Fact]
        public void ShouldHaveValidationError_WhenPasswordHasNoUppercaseLetter()
        {
            var model = new RegisterRequestDTO("test@example.com", "weakpassword1!");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password must contain at least one uppercase letter.");
        }

        [Fact]
        public void ShouldHaveValidationError_WhenPasswordHasNoLowercaseLetter()
        {
            var model = new RegisterRequestDTO("test@example.com", "WEAKPASSWORD1!");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password must contain at least one lowercase letter.");
        }

        [Fact]
        public void ShouldHaveValidationError_WhenPasswordHasNoNumber()
        {
            var model = new RegisterRequestDTO("test@example.com", "StrongPass!");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password must contain at least one number.");
        }

        [Fact]
        public void ShouldHaveValidationError_WhenPasswordHasNoSpecialCharacter()
        {
            var model = new RegisterRequestDTO("test@example.com", "StrongPass1");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password must contain at least one special character (@!#$%^&()_+-).");
        }
    }
}