import {
    urlWith,
    useGet,
    usePostData,
    usePutData,
    useDelete
} from "../../client"
import {
    useSystemSetting,
    useSystemSettingValue
} from "../../system-admin/setting/client"

let API = "/api/promotion"

export const usePromotionPayoutList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}
