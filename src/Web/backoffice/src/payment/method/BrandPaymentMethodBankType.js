import React from "react"
import { Spinner } from "bumbag"
import { getTrans, Trans, useLocale, useTrans } from "../../i18n"
import Select from "../../select/Select"
import { usePaymentMethodList } from "./client"
import { useBrandPaymentList } from "../../system/brandpayments/client"
import BrandPaymentTransferType from "../../system/brandpayments/BrandPaymentTransferType"
import { PaymentMethodSelect } from "./PaymentMethod"
import { PaymentMethodBankTypeSelect } from "./PaymentMethodBankType"

export const BrandPaymentMethodBankTypeSelect = ({
    placeholder,
    brandId,
    transferType = BrandPaymentTransferType.Withdrawal,
    ...props
}) => {
    let defaultPlaceholder = useTrans({
        zh: "支付类型",
        en: "Payment Type"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let { isLoading, data, error } = useBrandPaymentList(
        brandId
            ? {
                  brandId,
                  transferType,
                  showAll: true
              }
            : undefined
    )
    let range = data?.items?.map(({ paymentMethodId }) => paymentMethodId)

    return (
        <PaymentMethodBankTypeSelect
            isLoading={isLoading}
            range={range}
            {...props}
        />
    )
}
