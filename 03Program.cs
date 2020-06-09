using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
namespace BlobDemo
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello Azure Blob storage DEMO!");
            //예제를 비동기적으로 실행하고, 수행전에  결과를 기다린다.
            ProcessAsync().GetAwaiter().GetResult();
        }
        private static async Task ProcessAsync()
        {
            // 연결문자열 붙여 넣기
            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=az204blobdemo18332;AccountKey=EkePGUPqg/Jd7e9ovNpylnRj4/HS9a2fOU1adGxbf2CvCf4FVH2FhgSGXp79bDUG1LWL3A6GNkFfESaUsYexEg==;EndpointSuffix=core.windows.net";
            // 연결문자열 유효성 확인
            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                Console.WriteLine("연결문자열이 유효합니다..\r\n");
                // DEMO코드블럭 - 시작

                // CloudBlobClient 인스턴스 만들기 ###################################
                CloudBlobClient  cloudBlobClient = storageAccount.CreateCloudBlobClient();
                // 컨테이너 만들기
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("demoblobs" + Guid.NewGuid().ToString());
                await cloudBlobContainer.CreateAsync();
                Console.WriteLine("컨테이너가 만들어 졌습니다.");
                //Console.WriteLine("Press 'Enter' to continue.");
                //Console.ReadLine();

                // Blob 업로드 하기 ############################
                // 내문서 폴더에 파일 생성
                string localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string localFileName = "BlobDemo_" + Guid.NewGuid().ToString() + ".txt";
                string sourceFile = Path.Combine(localPath, localFileName);

                // 파일에 텍스트 쓰기
                File.WriteAllText(sourceFile, "Hello, World!");
                Console.WriteLine("\r\nTemp file = {0}", sourceFile);
                Console.WriteLine("Blob storage에 blob 업로드 '{0}'", localFileName);

                // Blob 주소에 대한 참조를 가져온 다음 파일을 Blob에 업로드
                // BLOB 이름으로 localFileName 을 사용하십시오.
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(localFileName);
                await cloudBlockBlob.UploadFromFileAsync(sourceFile);
                Console.WriteLine("\r\n Portal에서 blob일 업로드 완료");
                //Console.WriteLine("Press 'Enter' to continue.");
                //Console.ReadLine();


                // 컨테이너에 blob 목록 ##################################
                Console.WriteLine("컨테이너 안에 blob목록");
                BlobContinuationToken blobContinuationToken = null;
                do
                {
                    var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                    // 목록 호출에서 리턴 된 연속 토큰의 값을 가져온다.
                    blobContinuationToken = results.ContinuationToken;
                    foreach (IListBlobItem item in results.Results)
                    {
                        Console.WriteLine(item.Uri);
                    }
                } while (blobContinuationToken != null); // 연속 토큰이 null이 아닐 동안 loop를 돈다.

                Console.WriteLine("\r\n 콘솔에 출력된 목록과 포털 목록을 비교");
                //Console.WriteLine("Press 'Enter' to continue.");
                //Console.ReadLine();


                // blob 다운로드 하기 #######################################
                // 앞에서 만든 참조를 사용하여 Blob을 로컬 파일로 다운로드합니다.
                // .txt 확장자 앞에 "_DOWNLOADED"문자열을 추가해서 다운로드
                // MyDocuments에서 두 파일을 모두 볼 수 있습니다.
                string destinationFile = sourceFile.Replace(".txt", "_DOWNLOADED.txt");
                Console.WriteLine("Downloading blob to {0}", destinationFile);
                await cloudBlockBlob.DownloadToFileAsync(destinationFile, FileMode.Create);
                Console.WriteLine("\r\nLocate the local file to verify it was downloaded.");
                Console.WriteLine("Press 'Enter' to continue.");
                //Console.ReadLine();

                // 리소스 정리 #####################################
                // 생성한 리소스 정리
                Console.WriteLine("Press the 'Enter' key to delete the example files " + "and example container.");
                //Console.ReadLine();

                // 리소스를 정리합니다. 컨테이너와 수개의 임시 파일을 모두 지웁니다.
                Console.WriteLine("컨테이너 지우기");
                if (cloudBlobContainer != null)
                {
                await cloudBlobContainer.DeleteIfExistsAsync();
                }
                Console.WriteLine("소스파일과 다운로드 받은 파일 지우기\r\n");
                File.Delete(sourceFile);
                File.Delete(destinationFile);

                // DEMO코드블럭 - 끝
            }
            else
            {
                // 연결문자열이 유효하지 않은 경우에는 환경 변수를 설정 해야 한다는것을 알려준다.
                Console.WriteLine("storageConnectionString 변수에 유효한 연결 문자열이 정의되지 않았습니다.");
                Console.WriteLine("enter를 눌러 종료합니다.");
                Console.ReadLine();
            }    
        }
    }
}
