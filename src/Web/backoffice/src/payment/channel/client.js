import {
    urlWith,
    useGet,
    usePostData,
    usePutData,
    useDelete
} from "../../client"

let API = "/api/PaymentChannel"

const CodeTranPairs = []

export const usePaymentChannelList = () => {
    let { data, ...rest } = useGet(API)

    let items = []
    if (data?.data?.length > 0) {
        items = data?.data?.map?.((item) => {
            let [code, trans] =
                CodeTranPairs.find(([code, trans]) => code === item.code) ?? []

            return {
                ...item,
                trans: trans
            }
        })
    }

    return {
        data: items,
        ...rest
    }
}
