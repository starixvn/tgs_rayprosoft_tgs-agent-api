import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import { Box, Button, Set, Text, useModalState } from "bumbag"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update } from "./menu"
import Disclosure from "../../dialog/Disclosure"
import { Trans, useTrans } from "../../i18n"
import FormDialog from "../../form/FormDialog"

export const PromotionManualPayoutDisclosure = ({
    brandId,
    record,
    onSubmitted
}) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([Update])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog}>
                <Trans zh="手动派发" en="Manual Payout" />
            </Disclosure>
            {dialog.visible && (
                <ManualPayoutDialog
                    brandId={brandId}
                    record={record}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const ManualPayoutDialog = ({ brandId, record, onSubmitted, dialog }) => {
    let title = useTrans({
        zh: "手动派发",
        en: "Manual Payout"
    })

    return (
        <FormDialog title={title} dialog={dialog}>
            TODO: form body
        </FormDialog>
    )
}
