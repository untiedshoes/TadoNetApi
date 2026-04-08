using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Application.Services;
using TadoNetApi.Domain.Interfaces;
using Xunit;

namespace TadoNetApi.Tests.Application.Services
{
    public class HomeAppServiceTests
    {
        /// <summary>
        /// Tests that <see cref="HomeAppService.GetHomeAsync"/> returns the home from the domain service.
        /// </summary>
        [Fact(DisplayName = "GetHomeAsync returns home")]
        public async Task GetHomeAsync_ReturnsHome()
        {
            // Arrange
            var expectedHome = new House
            {
                Id = 1,
                Name = "My Home",
                DateTimeZone = "Europe/London",
                TemperatureUnit = "C",
                InstallationCompleted = true,
                ChristmasModeEnabled = false,
                IncidentDetection = new IncidentDetection
                {
                    Enabled = true,
                    Supported = true
                },
                ContactDetails = new ContactDetails(),
                Address = new Address(),
                Geolocation = new Geolocation()
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetHomeAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedHome);

            var service = new HomeAppService(mockHomeService.Object);

            // Act
            var home = await service.GetHomeAsync(1, CancellationToken.None);

            // Assert
            Assert.NotNull(home);
            Assert.Equal(1, home?.Id);
            Assert.Equal("My Home", home?.Name);
            Assert.True(home?.IncidentDetection?.Enabled);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.GetHomeStateAsync"/> returns the home state from the domain service.
        /// </summary>
        [Fact(DisplayName = "GetHomeStateAsync returns state")]
        public async Task GetHomeStateAsync_ReturnsState()
        {
            // Arrange
            var expectedState = new HomeState
            {
                Presence = "HOME"
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetHomeStateAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedState);

            var service = new HomeAppService(mockHomeService.Object);

            // Act
            var state = await service.GetHomeStateAsync(1, CancellationToken.None);

            // Assert
            Assert.NotNull(state);
            Assert.Equal("HOME", state?.Presence);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.GetUsersAsync"/> returns the users from the domain service.
        /// </summary>
        [Fact(DisplayName = "GetUsersAsync returns users")]
        public async Task GetUsersAsync_ReturnsUsers()
        {
            // Arrange
            var expectedUsers = new List<User>
            {
                new() { Id = "user-1", Name = "Alice Example", Email = "alice@example.com" },
                new() { Id = "user-2", Name = "Bob Example", Email = "bob@example.com" }
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetUsersAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedUsers);

            var service = new HomeAppService(mockHomeService.Object);

            // Act
            var users = await service.GetUsersAsync(1, CancellationToken.None);

            // Assert
            Assert.Equal(2, users.Count);
            Assert.Equal("Alice Example", users[0].Name);
            Assert.Equal("Bob Example", users[1].Name);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.GetAirComfortAsync"/> returns the air comfort payload from the domain service.
        /// </summary>
        [Fact(DisplayName = "GetAirComfortAsync returns air comfort")]
        public async Task GetAirComfortAsync_ReturnsAirComfort()
        {
            // Arrange
            var expectedAirComfort = new AirComfort
            {
                Freshness = new AirComfortFreshness
                {
                    Value = "FRESH"
                },
                AcPoweredOn = false,
                Comfort =
                [
                    new AirComfortComfort
                    {
                        RoomId = 1,
                        TemperatureLevel = "COMFY",
                        HumidityLevel = "COMFY",
                        Coordinate = new AirComfortCoordinate
                        {
                            Radial = 0.22,
                            Angular = 76
                        }
                    }
                ]
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetAirComfortAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAirComfort);

            var service = new HomeAppService(mockHomeService.Object);

            // Act
            var airComfort = await service.GetAirComfortAsync(1, CancellationToken.None);

            // Assert
            Assert.NotNull(airComfort);
            Assert.Equal("FRESH", airComfort.Freshness?.Value);
            Assert.False(airComfort.AcPoweredOn);
            Assert.Single(airComfort.Comfort!);
            Assert.Equal(1, airComfort.Comfort![0].RoomId);
            Assert.Equal(76, airComfort.Comfort[0].Coordinate?.Angular);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.GetInstallationsAsync"/> returns the installations from the domain service.
        /// </summary>
        [Fact(DisplayName = "GetInstallationsAsync returns installations")]
        public async Task GetInstallationsAsync_ReturnsInstallations()
        {
            var expectedInstallations = new List<Installation>
            {
                new()
                {
                    Id = 101,
                    CurrentType = "HEATING",
                    Revision = 7,
                    State = "COMPLETED",
                    Devices =
                    [
                        new Device { SerialNo = "VA1234567890", ShortSerialNo = "VA1234567890" }
                    ]
                }
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetInstallationsAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedInstallations);

            var service = new HomeAppService(mockHomeService.Object);

            var installations = await service.GetInstallationsAsync(1, CancellationToken.None);

            Assert.Single(installations);
            Assert.Equal(101, installations[0].Id);
            Assert.Equal("HEATING", installations[0].CurrentType);
            Assert.Single(installations[0].Devices);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.GetInstallationAsync"/> returns the installation from the domain service.
        /// </summary>
        [Fact(DisplayName = "GetInstallationAsync returns installation")]
        public async Task GetInstallationAsync_ReturnsInstallation()
        {
            var expectedInstallation = new Installation
            {
                Id = 101,
                CurrentType = "HEATING",
                Revision = 7,
                State = "COMPLETED",
                Devices =
                [
                    new Device { SerialNo = "VA1234567890", ShortSerialNo = "VA1234567890" }
                ]
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetInstallationAsync(1, 101, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedInstallation);

            var service = new HomeAppService(mockHomeService.Object);

            var installation = await service.GetInstallationAsync(1, 101, CancellationToken.None);

            Assert.NotNull(installation);
            Assert.Equal(101, installation?.Id);
            Assert.Equal("COMPLETED", installation?.State);
            Assert.Single(installation?.Devices!);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.GetInvitationsAsync"/> returns the invitations from the domain service.
        /// </summary>
        [Fact(DisplayName = "GetInvitationsAsync returns invitations")]
        public async Task GetInvitationsAsync_ReturnsInvitations()
        {
            var expectedInvitations = new List<Invitation>
            {
                new()
                {
                    Token = "token-1",
                    Email = "invitee@example.com",
                    Inviter = new InvitationInviter
                    {
                        Name = "Alice Example",
                        Email = "alice@example.com"
                    }
                }
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetInvitationsAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedInvitations);

            var service = new HomeAppService(mockHomeService.Object);

            var invitations = await service.GetInvitationsAsync(1, CancellationToken.None);

            Assert.Single(invitations);
            Assert.Equal("token-1", invitations[0].Token);
            Assert.Equal("invitee@example.com", invitations[0].Email);
            Assert.Equal("Alice Example", invitations[0].Inviter?.Name);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.GetIncidentDetectionAsync"/> returns the incident detection payload from the domain service.
        /// </summary>
        [Fact(DisplayName = "GetIncidentDetectionAsync returns incident detection")]
        public async Task GetIncidentDetectionAsync_ReturnsIncidentDetection()
        {
            // Arrange
            var expectedIncidentDetection = new IncidentDetection
            {
                Enabled = true,
                Supported = true
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetIncidentDetectionAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedIncidentDetection);

            var service = new HomeAppService(mockHomeService.Object);

            // Act
            var incidentDetection = await service.GetIncidentDetectionAsync(1, CancellationToken.None);

            // Assert
            Assert.NotNull(incidentDetection);
            Assert.True(incidentDetection.Enabled);
            Assert.True(incidentDetection.Supported);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.GetHeatingCircuitsAsync"/> returns the heating circuits from the domain service.
        /// </summary>
        [Fact(DisplayName = "GetHeatingCircuitsAsync returns heating circuits")]
        public async Task GetHeatingCircuitsAsync_ReturnsHeatingCircuits()
        {
            var expectedCircuits = new List<HeatingCircuit>
            {
                new() { Number = 1, DriverSerialNo = "BR3209250550", DriverShortSerialNo = "BR3209250550" }
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetHeatingCircuitsAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCircuits);

            var service = new HomeAppService(mockHomeService.Object);

            var circuits = await service.GetHeatingCircuitsAsync(1, CancellationToken.None);

            Assert.Single(circuits);
            Assert.Equal(1, circuits[0].Number);
            Assert.Equal("BR3209250550", circuits[0].DriverSerialNo);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.GetHeatingSystemAsync"/> returns the heating system from the domain service.
        /// </summary>
        [Fact(DisplayName = "GetHeatingSystemAsync returns heating system")]
        public async Task GetHeatingSystemAsync_ReturnsHeatingSystem()
        {
            var expectedHeatingSystem = new HeatingSystem
            {
                Boiler = new Boiler
                {
                    Present = true,
                    Id = 2699,
                    Found = true
                },
                UnderfloorHeating = new UnderfloorHeating
                {
                    Present = false
                }
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetHeatingSystemAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedHeatingSystem);

            var service = new HomeAppService(mockHomeService.Object);

            var heatingSystem = await service.GetHeatingSystemAsync(1, CancellationToken.None);

            Assert.NotNull(heatingSystem);
            Assert.True(heatingSystem.Boiler?.Present);
            Assert.Equal(2699, heatingSystem.Boiler?.Id);
            Assert.False(heatingSystem.UnderfloorHeating?.Present);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.GetFlowTemperatureOptimisationAsync"/> returns the flow-temperature optimisation payload from the domain service.
        /// </summary>
        [Fact(DisplayName = "GetFlowTemperatureOptimisationAsync returns flow temperature optimisation")]
        public async Task GetFlowTemperatureOptimisationAsync_ReturnsFlowTemperatureOptimisation()
        {
            var expectedFlowTemperatureOptimisation = new FlowTemperatureOptimisation
            {
                HasMultipleBoilerControlDevices = false,
                MaxFlowTemperature = 50,
                MaxFlowTemperatureConstraints = new FlowTemperatureOptimisationConstraints
                {
                    Min = 30,
                    Max = 80
                },
                AutoAdaptation = new FlowTemperatureOptimisationAutoAdaptation
                {
                    Enabled = true,
                    MaxFlowTemperature = 45
                },
                OpenThermDeviceSerialNumber = "BR1234567890"
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetFlowTemperatureOptimisationAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedFlowTemperatureOptimisation);

            var service = new HomeAppService(mockHomeService.Object);

            var flowTemperatureOptimisation = await service.GetFlowTemperatureOptimisationAsync(1, CancellationToken.None);

            Assert.NotNull(flowTemperatureOptimisation);
            Assert.False(flowTemperatureOptimisation.HasMultipleBoilerControlDevices);
            Assert.Equal(50, flowTemperatureOptimisation.MaxFlowTemperature);
            Assert.Equal(30, flowTemperatureOptimisation.MaxFlowTemperatureConstraints?.Min);
            Assert.Equal(80, flowTemperatureOptimisation.MaxFlowTemperatureConstraints?.Max);
            Assert.True(flowTemperatureOptimisation.AutoAdaptation?.Enabled);
            Assert.Equal("BR1234567890", flowTemperatureOptimisation.OpenThermDeviceSerialNumber);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.SetHomePresenceAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "SetHomePresenceAsync passes through to domain service")]
        public async Task SetHomePresenceAsync_PassesThroughToDomainService()
        {
            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.SetHomePresenceAsync(1, "HOME", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new HomeAppService(mockHomeService.Object);

            await service.SetHomePresenceAsync(1, "HOME", CancellationToken.None);

            mockHomeService.Verify(s => s.SetHomePresenceAsync(1, "HOME", It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.SendInvitationAsync"/> returns the invitation from the domain service.
        /// </summary>
        [Fact(DisplayName = "SendInvitationAsync returns invitation")]
        public async Task SendInvitationAsync_ReturnsInvitation()
        {
            var expectedInvitation = new Invitation
            {
                Token = "token-1",
                Email = "invitee@example.com"
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.SendInvitationAsync(1, "invitee@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedInvitation);

            var service = new HomeAppService(mockHomeService.Object);

            var invitation = await service.SendInvitationAsync(1, "invitee@example.com", CancellationToken.None);

            Assert.Equal("token-1", invitation.Token);
            Assert.Equal("invitee@example.com", invitation.Email);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.DeleteInvitationAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "DeleteInvitationAsync passes through to domain service")]
        public async Task DeleteInvitationAsync_PassesThroughToDomainService()
        {
            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.DeleteInvitationAsync(1, "token-1", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new HomeAppService(mockHomeService.Object);

            await service.DeleteInvitationAsync(1, "token-1", CancellationToken.None);

            mockHomeService.Verify(s => s.DeleteInvitationAsync(1, "token-1", It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.ResendInvitationAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "ResendInvitationAsync passes through to domain service")]
        public async Task ResendInvitationAsync_PassesThroughToDomainService()
        {
            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.ResendInvitationAsync(1, "token-1", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new HomeAppService(mockHomeService.Object);

            await service.ResendInvitationAsync(1, "token-1", CancellationToken.None);

            mockHomeService.Verify(s => s.ResendInvitationAsync(1, "token-1", It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.ResetHomePresenceAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "ResetHomePresenceAsync passes through to domain service")]
        public async Task ResetHomePresenceAsync_PassesThroughToDomainService()
        {
            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.ResetHomePresenceAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new HomeAppService(mockHomeService.Object);

            await service.ResetHomePresenceAsync(1, CancellationToken.None);

            mockHomeService.Verify(s => s.ResetHomePresenceAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.SetAwayRadiusInMetersAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "SetAwayRadiusInMetersAsync passes through to domain service")]
        public async Task SetAwayRadiusInMetersAsync_PassesThroughToDomainService()
        {
            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.SetAwayRadiusInMetersAsync(1, 400, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new HomeAppService(mockHomeService.Object);

            await service.SetAwayRadiusInMetersAsync(1, 400, CancellationToken.None);

            mockHomeService.Verify(s => s.SetAwayRadiusInMetersAsync(1, 400, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.SetIncidentDetectionAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "SetIncidentDetectionAsync passes through to domain service")]
        public async Task SetIncidentDetectionAsync_PassesThroughToDomainService()
        {
            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.SetIncidentDetectionAsync(1, true, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new HomeAppService(mockHomeService.Object);

            await service.SetIncidentDetectionAsync(1, true, CancellationToken.None);

            mockHomeService.Verify(s => s.SetIncidentDetectionAsync(1, true, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.SetHomeDetailsAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "SetHomeDetailsAsync passes through to domain service")]
        public async Task SetHomeDetailsAsync_PassesThroughToDomainService()
        {
            var homeDetails = new House
            {
                Name = "My Home",
                ContactDetails = new ContactDetails { Name = "Jane Doe", Email = "jane@example.com", Phone = "+441234567890" },
                Address = new Address { AddressLine1 = "1 Test Street", ZipCode = "SW1A 1AA", City = "London", Country = "GBR" },
                Geolocation = new Geolocation { Latitude = 51.501, Longitude = -0.141 }
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.SetHomeDetailsAsync(1, homeDetails, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new HomeAppService(mockHomeService.Object);

            await service.SetHomeDetailsAsync(1, homeDetails, CancellationToken.None);

            mockHomeService.Verify(s => s.SetHomeDetailsAsync(1, homeDetails, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="HomeAppService.SetFlowTemperatureOptimisationAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "SetFlowTemperatureOptimisationAsync passes through to domain service")]
        public async Task SetFlowTemperatureOptimisationAsync_PassesThroughToDomainService()
        {
            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.SetFlowTemperatureOptimisationAsync(1, 55, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new HomeAppService(mockHomeService.Object);

            await service.SetFlowTemperatureOptimisationAsync(1, 55, CancellationToken.None);

            mockHomeService.Verify(s => s.SetFlowTemperatureOptimisationAsync(1, 55, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}