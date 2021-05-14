import {
    urlWith,
    useGet,
    usePostData,
    usePutData,
    useDelete
} from "../../client"

let API = "/api/PaymentMethod"

let CodeTranPairs = [
    [
        "Crypto",
        {
            zh: "数字货币",
            en: "Crypto"
        }
    ],
    [
        "BankTransfer",
        {
            zh: "银行卡转账",
            en: "Bank Transfer"
        }
    ],
    [
        "WeChatPay",
        {
            zh: "微信支付",
            en: "WeChat Pay"
        }
    ]
]

export const usePaymentMethodList = () => {
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
