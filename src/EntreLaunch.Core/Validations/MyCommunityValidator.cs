namespace EntreLaunch.Validations;

public class PostWithMediaCreateValidator : AbstractValidator<PostWithMediaCreateDto>
{
    public PostWithMediaCreateValidator()
    {
        RuleFor(x => x.Text).MustNotBeDefault();
        RuleFor(x => x.Text).MustHaveLengthInRange(10, 2500);
    }
}

public class TextPostCreateValidator : AbstractValidator<TextPostCreateDto>
{
    public TextPostCreateValidator()
    {
        RuleFor(x => x.Text).MustNotBeDefault();
        RuleFor(x => x.Text).MustHaveLengthInRange(10, 2500);
    }
}

//public class PostEntreLaunchdateValidator : AbstractValidator<PostEntreLaunchdateDto>
//{
//    public PostEntreLaunchdateValidator()
//    {
//        RuleFor(x => x.UserId).MustNotBeDefault();

//        //RuleFor(x => x.Text).MustHaveLengthInRange(10, 2500);
//    }
//}

/*******Comment********/

public class CommentCreateValidator : AbstractValidator<CommentCreateDto>
{
    public CommentCreateValidator()
    {
        RuleFor(x => x.PostId).MustNotBeDefault();

        RuleFor(x => x.UserId).MustNotBeDefault();

        RuleFor(x => x.Content).MustNotBeDefault();
        RuleFor(x => x.Content).MustHaveLengthInRange(2, 500);
    }
}

public class CommentEntreLaunchdateValidator : AbstractValidator<CommentEntreLaunchdateDto>
{
    public CommentEntreLaunchdateValidator()
    {
        RuleFor(x => x.Content).MustHaveLengthInRange(2, 500);
    }
}

/*******Media********/
public class MediaCreateValidator : AbstractValidator<MediaCreateDto>
{
    public MediaCreateValidator()
    {
        RuleFor(x => x.Url).MustNotBeDefault();
        RuleFor(x => x.Url).MustBeValidAttachment();
    }
}

//public class MediaEntreLaunchdateValidator : AbstractValidator<MediaEntreLaunchdateDto>
//{
//    public MediaEntreLaunchdateValidator()
//    {
//        RuleFor(x => x.PostId).MustNotBeDefault();

//        //RuleFor(x => x.Url).MustBeValidAttachment();
//    }
//}
