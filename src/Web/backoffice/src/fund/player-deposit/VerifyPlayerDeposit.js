import React from "react"
import { Spinner, useModalState } from "bumbag"
import { getTrans, Trans, useLocale, useTrans } from "../../i18n"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update } from "./menu"
import { Disclosure, VerifyDisclosure } from "../../dialog/Disclosure"
import { FormDialog } from "../../form/FormDialog"

export const VerifyPlayerDepositDisclosure = ({
    brandId,
    formId,
    onSubmitted
}) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([Update])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <VerifyDisclosure {...dialog} />
            <VerifyPlayerDepositDialog
                brandId={brandId}
                formId={formId}
                dialog={dialog}
            />
        </>
    )
}

const VerifyPlayerDepositDialog = ({ dialog, ...props }) => {
    let title = useTrans({
        zh: "充值审核",
        en: "Verify Deposit"
    })

    return (
        <FormDialog title={title} dialog={dialog}>
            TODO
        </FormDialog>
    )
}
