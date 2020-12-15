using FluentValidation.TestHelper;
using NUnit.Framework;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.Brand;
using TGS.Services.BackOffice.Validation.Brand;

namespace TGS.Tests.BackendOffice.UnitTests.Validators.Brand
{
    [TestFixture]
    public class BrandGameProviderModelValidatorTests
    {
        [SetUp]
        public void Setup()
        {
            _validator = new BrandGameProviderModelValidator();
        }

        private BrandGameProviderModelValidator _validator;

        [Test]
        public void Should_have_error_when_client_id_is_null()
        {
            var model = new BrandGameProviderModel
            {
                Configuration = new Configuration
                {
                    ClientId = null
                }
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.Configuration.ClientId)
                .WithErrorMessage(Services.BackOffice.Application.Resources.Brand.ClientIdRequired);
        }

        [Test]
        public void Should_have_error_when_secret_key_is_null()
        {
            var model = new BrandGameProviderModel
            {
                Configuration = new Configuration
                {
                    SecretKey = null
                }
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.Configuration.SecretKey)
                .WithErrorMessage(Services.BackOffice.Application.Resources.Brand.SecretKeyRequired);
        }

        [Test]
        public void Should_have_error_when_status_is_not_in_enums()
        {
            var model = new BrandGameProviderModel
            {
                Configuration = new Configuration
                {
                    Status = (Status) 5
                }
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.Configuration.Status)
                .WithErrorMessage(Services.BackOffice.Application.Resources.Brand.StatusRequired);
        }

        [Test]
        public void Should_have_error_when_desktop_endpoint_is_null()
        {
            var model = new BrandGameProviderModel
            {
                Configuration = new Configuration
                {
                    DesktopEndpoint = null
                }
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.Configuration.DesktopEndpoint)
                .WithErrorMessage(Services.BackOffice.Application.Resources.Brand.DesktopEndpointRequired);
        }

        [Test]
        public void Should_have_error_when_mobile_endpoint_is_null()
        {
            var model = new BrandGameProviderModel
            {
                Configuration = new Configuration
                {
                    MobileEndpoint = null
                }
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.Configuration.MobileEndpoint)
                .WithErrorMessage(Services.BackOffice.Application.Resources.Brand.MobileEndpointRequired);
        }

        [Test]
        public void Should_have_error_when_configuration_data_is_null()
        {
            var model = new BrandGameProviderModel
            {
                Configuration = new Configuration
                {
                    ConfigurationData = null
                }
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.Configuration.ConfigurationData)
                .WithErrorMessage(Services.BackOffice.Application.Resources.Brand.ConfigurationDataRequired);
        }
    }
}