import React from "react"
import {
    Spinner,
    useModalState,
    FieldStack,
    Box,
    Text,
    Dialog,
    Modal,
    Heading,
    Link,
    Flex,
    Set
} from "bumbag"
import {
    FormattedDateTime,
    FormattedNumberDecimal,
    getTrans,
    Trans,
    useLocale,
    useTrans
} from "../../i18n"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update, View } from "./menu"
import {
    Disclosure,
    VerifyDisclosure,
    ViewDisclosure
} from "../../dialog/Disclosure"
import { FormDialog } from "../../form/FormDialog"
import AlertDialog from "../../dialog/Alert"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import { useFundWithdrawForm } from "./client"
import Separator from "../../layout/Separator"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import { PaymentMethodLabel } from "../../payment/method/PaymentMethod"
import { FundReviewStatusLabel } from "../FundReviewStatus"
import Select from "../../select/Select"

export const WithdrawMethodSelect = ({ ...props }) => {
    let options = [
        {
            label: useTrans({
                zh: "三方出款",
                en: "Third Party Payment"
            }),
            value: "ThirdPartyPayment"
        }
    ]

    return <Select options={options} {...props} />
}
