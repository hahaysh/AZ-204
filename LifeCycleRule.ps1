# 리소스그룹과 SA 선언
$rgname = "hahaysh02rg"
$accountName = "hahaysh02stad"

# 정책 작업 집합 만들기
$action = Add-AzStorageAccountManagementPolicyAction -BaseBlobAction Delete -daysAfterModificationGreaterThan 2555
$action = Add-AzStorageAccountManagementPolicyAction -InputObject $action -BaseBlobAction TierToArchive -daysAfterModificationGreaterThan 90
$action = Add-AzStorageAccountManagementPolicyAction -InputObject $action -BaseBlobAction TierToCool -daysAfterModificationGreaterThan 30
$action = Add-AzStorageAccountManagementPolicyAction -InputObject $action -SnapshotAction Delete -daysAfterCreationGreaterThan 90

# 필터 생성
$filter = New-AzStorageAccountManagementPolicyFilter -PrefixMatch ab,cd

# 새 규칙 생성
$rule1 = New-AzStorageAccountManagementPolicyRule -Name hahaysh02rule -Action $action -Filter $filter

# 정책 설정
$policy = Set-AzStorageAccountManagementPolicy -ResourceGroupName $rgname -StorageAccountName $accountName -Rule $rule1
