using AutoMapper;
namespace EntreLaunch.Plugin.SmartExam.Services
{
    public class ExamService : IExamService
    {
        private readonly PgDbContext _dbContext;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        public ExamService(PgDbContext pgDbContext, IMapper mapper)
        {
            _httpClient = new HttpClient();
            _dbContext = pgDbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Create an Exam and add it to the database.
        /// </summary>
        private async Task<ExamDetailsDto> CreateExamAsync(ExamCreateDto examDto)
        {
            var examEntity = new Exam
            {
                Name = examDto.Name,
                Type = examDto.Type,
                Description = examDto.Description,
                MinMark = examDto.MinMark,
                MaxMark = examDto.MaxMark,
                DurationInMinutes = examDto.DurationInMinutes,
                ParentEntityType = examDto.ParentEntityType ?? 0,
                CourseId = examDto.CourseId,
                LessonId = examDto.LessonId,
                PathId = examDto.PathId
            };


            _dbContext.Exams.Add(examEntity);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<ExamDetailsDto>(examEntity);
        }

        /// <summary>
        /// Create a question and add it to the database.
        /// </summary>
        private async Task<List<QuestionDetailsDto>> CreateQuestionsAsync(QuestionCreateDto questionDto)
        {
            var questionEntity = new Question
            {
                ExamId = questionDto.ExamId,
                Text = questionDto.Text,
                Mark = questionDto.Mark
            };

            _dbContext.Questions.Add(questionEntity);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<List<QuestionDetailsDto>>(questionEntity);
        }

        /// <summary>
        /// Create a answer and add it to the database.
        /// </summary>
        private async Task<List<AnswerDetailsDto>> CreateAnswersAsync(AnswerCreateDto answereDto)
        {
            var answerEntity = new Answer
            {
                QuestionId = answereDto.QuestionId,
                Text = answereDto.Text!,
                IsCorrect = answereDto.IsCorrect ?? false,
            };

            _dbContext.Answers.Add(answerEntity);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<List<AnswerDetailsDto>>(answerEntity);
        }

        /// <summary>
        /// Generate questions from the AI.
        /// </summary>
        public async Task<string> GenerateQuestionsAsync(AIRequestDataDto aIRequestDataDto)
        {
            // Prepare the request data
            var requestData = new
            {
                inputs = new
                {
                    ExamTopic = aIRequestDataDto.ExamTopic,
                    Questions_number = aIRequestDataDto.QuestionsNumber,
                    minMark = aIRequestDataDto.MinMark,
                    maxMark = aIRequestDataDto.MaxMark,
                    lang = aIRequestDataDto.language
                },
                response_mode = "blocking",
                user = 123
            };

            // Convert the Body to JSON
            string requestJson = JsonSerializer.Serialize(requestData);

            // Setting EntreLaunch the POST message and attaching the JSON to the content
            var requestMessage = new HttpRequestMessage(HttpMethod.Post,
                "https://api.dify.ai/v1/workflows/run")
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };

            // Add authentication header
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "app-Nk5x9V4xgolZXjUz4KQUjHn4");

            try
            {
                // Sending a request
                var response = await _httpClient.SendAsync(requestMessage);
                string responseContent = await response.Content.ReadAsStringAsync();

                // check if the request was successful
                if (!response.IsSuccessStatusCode)
                {
                    return $"❌ API Error: {response.StatusCode}, Details: {responseContent}";
                }

                // Analyze the response
                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;

                //extract the text from the API response
                if (root.TryGetProperty("data", out var dataElement) &&
                    dataElement.TryGetProperty("outputs", out var outputsElement) &&
                    outputsElement.TryGetProperty("text", out var textElement))
                {
                    string rawAnswer = textElement.GetString() ?? "";
                    string cleanedAnswer = ExtractCleanJson(rawAnswer);

                    //// 🔹 تحويل النص إلى كائن ExamDto
                    //var examData = JsonSerializer.Deserialize<ExamDtoWithQuestions>(cleanedAnswer, new JsonSerializerOptions
                    //{
                    //    PropertyNameCaseInsensitive = true
                    //});

                    //if (examData == null || examData.Questions == null || examData.Questions.Count == 0)
                    //{
                    //    return "⚠️ Error: Failed to parse questions from AI response.";
                    //}

                   
                    return cleanedAnswer;
                }
                else if (root.TryGetProperty("data", out var errorElement) &&
                    errorElement.TryGetProperty("outputs", out var outErorrElement) &&
                    outErorrElement.TryGetProperty("result", out var resultElement))
                {
                    string result = resultElement.GetString() ?? "";
                    string cleanedAnswer = ExtractCleanJson(result);

                    return $"⚠️ Error: {cleanedAnswer}";
                }
                else
                {
                    return "⚠️ Error: No 'text' field found in the API response.";
                }
            }
            catch (Exception ex)
            {
                return $"Error: An exception occurred. Details: {ex.Message}";
            }
        }


        public async Task<string> GenerateQuestionsFromFilesAsync(AIFileRequestDataDto aIFileRequestDataDto)
        {
            if (aIFileRequestDataDto.fileIds == null || aIFileRequestDataDto.fileIds.Count == 0)
            {
                return "❌ Error: No valid file IDs provided.";
            }

            var files = aIFileRequestDataDto.fileIds.Select(fileId => new
            {
                transfer_method = "local_file",
                EntreLaunchload_file_id = fileId,
                type = "document"
            }).ToList();

            // تحضير بيانات الطلب
            var requestData = new
            {
                inputs = new
                {
                    file = files,
                    Questions_number = aIFileRequestDataDto.QuestionsNumber,
                    minMark = aIFileRequestDataDto.MinMark,
                    maxMark = aIFileRequestDataDto.MaxMark,
                    lang = aIFileRequestDataDto.Language
                },
                response_mode = "blocking",
                user = 123
            };

            // تحويل الطلب إلى JSON
            string requestJson = JsonSerializer.Serialize(requestData);

            // إعداد الطلب HTTP POST
            var requestMessage = new HttpRequestMessage(HttpMethod.Post,
                "https://api.dify.ai/v1/workflows/run")
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };

            // إضافة مفتاح التحقق
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "app-Nk5x9V4xgolZXjUz4KQUjHn4");

            try
            {
                // إرسال الطلب
                var response = await _httpClient.SendAsync(requestMessage);
                string responseContent = await response.Content.ReadAsStringAsync();

                // التحقق من نجاح الطلب
                if (!response.IsSuccessStatusCode)
                {
                    return $"❌ API Error: {response.StatusCode}, Details: {responseContent}";
                }

                // تحليل الاستجابة
                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;

                // استخراج الأسئلة من المخرجات
                if (root.TryGetProperty("data", out var dataElement) &&
                    dataElement.TryGetProperty("outputs", out var outputsElement) &&
                    outputsElement.TryGetProperty("text", out var textElement))
                {
                    string rawAnswer = textElement.GetString() ?? "";
                    string cleanedAnswer = ExtractCleanJson(rawAnswer);

                   

                    return cleanedAnswer;
                }
                else
                {
                    return "⚠️ Error: No 'text' field found in the API response.";
                }
            }
            catch (Exception ex)
            {
                return $"Error: An exception occurred. Details: {ex.Message}";
            }
        }

        public async Task<List<string>> EntreLaunchloadFilesToDify(List<IFormFile> files, string userId)
        {
            if (files == null || files.Count == 0)
                throw new Exception("❌ Error: No files EntreLaunchloaded.");

            var EntreLaunchloadUrl = "https://api.dify.ai/v1/files/EntreLaunchload";
            var EntreLaunchloadedFileIds = new List<string>();

            foreach (var file in files)
            {
                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                content.Add(fileContent, "file", file.FileName);
                content.Add(new StringContent("document"), "type");  // ✅ تأكيد أن نوع الملف هو document
                content.Add(new StringContent(userId), "user");

                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, EntreLaunchloadUrl)
                {
                    Content = content
                };

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "app-Nk5x9V4xgolZXjUz4KQUjHn4");

                var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"❌ EntreLaunchload Error: {response.StatusCode}, Details: {responseContent}");

                using var doc = JsonDocument.Parse(responseContent);
                var fileId = doc.RootElement.GetProperty("id").GetString();

                if(fileId == null)
                {
                    throw new Exception("❌ Error: fileId is null.");
                }

                EntreLaunchloadedFileIds.Add(fileId!);
            }

            return EntreLaunchloadedFileIds;
        }


        //public async Task<ExamGradeResultDto> GradeExamAsync(int examId, List<QuestionAnswerDto> userAnswers)
        //{
        //    var exam = await _dbContext.Exams.Include(e => e.Questions)
        //        .FirstOrDefaultAsync(e => e.Id == examId);

        //    if (exam == null)
        //    {
        //        throw new Exception("Exam not found.");
        //    }

        //    var questions = exam.Questions;
        //    int totalQuestions = questions.Count;

        //    if (totalQuestions == 0)
        //        throw new Exception("No questions in this exam.");

        //    var result = new ExamGradeResultDto
        //    {
        //        ExamId = examId,
        //        TotalQuestions = totalQuestions,
        //        QuestionsDetail = new List<QuestionGradeDetailDto>()
        //    };

        //    int correctCount = 0;
        //    double totalScore = 0;

        //    // 3. المرور على الأسئلة ومقارنة الإجابات
        //    foreach (var question in questions)
        //    {
        //        // إيجاد إجابة المستخدم المطابقة لمعرف السؤال الحالي
        //        var userAnswerDto = userAnswers.FirstOrDefault(a => a.QuestionId == question.Id);
        //        if (userAnswerDto == null)
        //        {
        //            // لم يقدّم المستخدم إجابة لهذا السؤال
        //            result.QuestionsDetail.Add(new QuestionGradeDetailDto
        //            {
        //                QuestionId = question.Id,
        //                UserAnswer = null,
        //                IsCorrect = false,
        //                QuestionMark = 1,
        //                AwardedMark = 0,
        //                CorrectAnswer = question.CorrectAnswer
        //            });
        //            continue;
        //        }

        //        // #### سيناريو الأسئلة متعددة الاختيارات ####
        //        // مقارنة نص الإجابة مع السؤال.CorrectAnswer
        //        var isCorrect = userAnswerDto.Answer?.Trim().Equals(question.CorrectAnswer?.Trim(),
        //                        StringComparison.OrdinalIgnoreCase) ?? false;

        //        double questionMark = 1; // يمكن جلب قيمة الدرجات من حقل Mark في الكيان أو أي منطقي محدد.
        //        double awardedMark = isCorrect ? questionMark : 0;

        //        // في حال الأسئلة المقالية، نضيف استدعاء لطريقة التصحيح بالـ AI (سنراها لاحقًا)
        //        // awardedMark = await _aiGradingService.GradeEssayAsync(question, userAnswerDto.Answer);

        //        if (isCorrect)
        //        {
        //            correctCount++;
        //            totalScore += awardedMark;
        //        }

        //        result.QuestionsDetail.Add(new QuestionGradeDetailDto
        //        {
        //            QuestionId = question.Id,
        //            UserAnswer = userAnswerDto.Answer,
        //            IsCorrect = isCorrect,
        //            QuestionMark = questionMark,
        //            AwardedMark = awardedMark,
        //            CorrectAnswer = question.CorrectAnswer
        //        });
        //    }

        //    // 4. تعبئة بقية البيانات
        //    result.CorrectCount = correctCount;
        //    result.WrongCount = totalQuestions - correctCount;
        //    result.TotalScore = totalScore;

        //    // 5. إمكانية تخزين نتيجة التصحيح في قاعدة بيانات
        //    // مثلاً إنشاء كيان ExamSubmission
        //    // var submission = new ExamSubmission { ... };
        //    // _context.ExamSubmissions.Add(submission);
        //    // await _context.SaveChangesAsync();

        //    // 6. إعادة النتيجة
        //    return result;
        //}

        // helper methods.

        /// <summary>
        /// Extract the text between ``json`` and ```.
        /// </summary>
        private string ExtractCleanJson(string input)
        {
            // Locate the JSON content within the raw string
            int startIndex = input.IndexOf("```json") + 7; // Skip "```json"
            int endIndex = input.LastIndexOf("```");

            if (startIndex >= 7 && endIndex > startIndex)
            {
                return input.Substring(startIndex, endIndex - startIndex).Trim();
            }

            // If no valid JSON markers are found, return the original input
            return input;
        }

        Task<ExamGradeResultDto> IExamService.GradeExamAsync(int examId, List<QuestionAnswerDto> userAnswers)
        {
            throw new NotImplementedException();
        }
    }
}
