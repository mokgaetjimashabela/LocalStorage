using System.Text.Json;

namespace LocalStorage;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileService profileService;
    private Profile userProfile;
    public ProfilePage()
	{
		InitializeComponent();
        profileService = new ProfileService();
        userProfile = new Profile();

		LoadProfile();
	}

    private async void LoadProfile()
    {
        userProfile = await profileService.LoadProfileAsync();
        UpdateUI();
    }

    private void UpdateUI()
    {
        nameEntry.Text = userProfile.Name;
        surnameEntry.Text = userProfile.Surname;
        emailEntry.Text = userProfile.Email;
        bioEditor.Text = userProfile.Bio;
        profileImage.Source = !string.IsNullOrEmpty(userProfile.ProfilePicture)
                                    ? ImageSource.FromFile(userProfile.ProfilePicture)
                                    : null;
    }

    private async void SaveProfile(object sender, EventArgs e)
    {
        userProfile.Name = nameEntry.Text;
        userProfile.Surname = surnameEntry.Text;
        userProfile.Email = emailEntry.Text;
        userProfile.Bio = bioEditor.Text;

        await profileService.SaveProfileAsync(userProfile);
    }
    private async void UploadPicture(object sender, EventArgs e)
    {
        var result = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images,
            PickerTitle = "Select Profile Picture"
        });

        if (result != null)
        {
            profileImage.Source = ImageSource.FromFile(result.FullPath);
            userProfile.ProfilePicture = result.FullPath;
        }
    }
}

// Backend: Model class for Profile
public class Profile
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Title { get; set; }
    public string ProfilePicture { get; set; } = string.Empty;
}

// Backend: Handles profile data management
public class ProfileService
{
    private const string FileName = "profile.json";
    private readonly string filePath = Path.Combine(FileSystem.AppDataDirectory, FileName);

    public async Task<Profile> LoadProfileAsync()
    {
        if (File.Exists(filePath))
        {
            string json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<Profile>(json) ?? new Profile();
        }
        return new Profile();
    }

    public async Task SaveProfileAsync(Profile profile)
    {
        string json = JsonSerializer.Serialize(profile, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }
}