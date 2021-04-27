import { urlWith, useGet, usePostData, usePutData, useDelete } from "../client"

let API = "/api/Payment"

let CodeTranPairs = [
    [
        "Manual",
        {
            zh: "TGS",
            en: "TGS"
        }
    ]
]

export const usePaymentList = () => {
    let { data, ...rest } = useGet(API)

    let items = []
    if (data?.data?.length > 0) {
        items = data?.data?.map((item) => {
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
