using System;
using System.Collections.Generic;
using System.Linq;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Events;

namespace DFDS.TeamService.WebApi.Features.Teams.Domain.Models
{
    public class Team : AggregateRoot<Guid>
    {
        private readonly List<Membership> _memberships = new List<Membership>();

        private Team()
        {
            
        }

        public Team(Guid id, string name, string department, IEnumerable<Membership> memberships) : base(id)
        {
            Name = name;
            Department = department;
            _memberships.AddRange(memberships);
        }

        public string Name { get; private set; }
        public void ChangeName(string newName)
        {
            Name = newName;
        }

        public string Department { get; private set; }
        public void ChangeDepartment(string newDepartment)
        {
            Department = newDepartment;
        }

        public IEnumerable<User> Members => _memberships.Select(x => x.User).Distinct();
        public IEnumerable<Membership> Memberships => _memberships;

        public User FindMemberById(string id)
        {
            return Members.SingleOrDefault(x => x.Id == id);
        }

        public void StartMembership(User user, MembershipType membershipType)
        {
            var membership = Membership.Start(user, membershipType);
            _memberships.Add(membership);

            RaiseEvent(new UserJoinedTeam(
                teamId: this.Id,
                userId: user.Id,
                userHasRole: membership.Type,
                startedDate: membership.StartedDate
            ));
        }

        public static Team Create(string name, string department)
        {
            var team = new Team(
                id: Guid.NewGuid(),
                name: name,
                department: department,
                memberships: Enumerable.Empty<Membership>()
            );

            team.RaiseEvent(new TeamCreated(
                teamId: team.Id,
                teamName: team.Name,
                department: team.Department
            ));

            return team;
        }
    }
}