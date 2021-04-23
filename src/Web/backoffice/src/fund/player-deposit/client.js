import {
    urlWith,
    useGet,
    usePostData,
    usePutData,
    useDelete
} from "../../client"

let API = "/api/deposit"

export const usePlayerDepositList = (brandId, searchParams) => {
    let url =
        brandId && searchParams
            ? urlWith(searchParams, `${API}/${brandId}`)
            : ""
    return useGet(url)
}
