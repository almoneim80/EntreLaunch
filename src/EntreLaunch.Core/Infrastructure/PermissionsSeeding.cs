namespace EntreLaunch.Infrastructure
{
    public class PermissionsSeeding
    {
        public static async Task SeedAdminPermissionsAsync(RoleManager<IdentityRole> roleManager)
        {
            // find the Admin role
            var adminRole = await roleManager.FindByNameAsync("Admin");
            if (adminRole == null)
            {
                adminRole = new IdentityRole("Admin");
                var result = await roleManager.CreateAsync(adminRole);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create Admin role: " +
                                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // get all permissions from the Permissions class
            var nestedTypes = typeof(Permissions).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

            // loop through all nested types
            foreach (var type in nestedTypes)
            {
                // get all public static string fields
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                 .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

                foreach (var field in fields)
                {
                    // check if the field has the AdminOnlyAttribute
                    var hasAdminOnly = field.GetCustomAttribute<AdminOnlyAttribute>() != null;
                    if (!hasAdminOnly)
                        continue;

                    // extract the permission value
                    var permissionValue = field.GetRawConstantValue() as string;
                    if (string.IsNullOrWhiteSpace(permissionValue))
                        continue;

                    // check if the Admin role already has the claim
                    var existingClaims = await roleManager.GetClaimsAsync(adminRole);
                    bool alreadyHasClaim = existingClaims.Any(c => c.Type == "Permission" && c.Value == permissionValue);
                    if (alreadyHasClaim)
                        continue; // skip if the Admin role already has the claim

                    // add the claim
                    var addResult = await roleManager.AddClaimAsync(adminRole, new System.Security.Claims.Claim("Permission", permissionValue));
                    if (!addResult.Succeeded)
                    {
                        throw new Exception($"Failed to add permission {permissionValue} to Admin role.");
                    }
                }
            }
        }
    }
}
