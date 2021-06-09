import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useLocation, useRouteMatch } from "react-router-dom"
import { Stack, Box, Text, Dialog, useModalState, Alert } from "bumbag"
import Disclosure from "../../dialog/Disclosure"
import { Trans, useTrans } from "../../i18n"
import Backdrop from "../../dialog/Backdrop"
import { WarningConfirmFormDialog } from "../../form/FormDialog"
import { useClearPlayerWageringRequirement } from "./client"
import AlertDialog from "../../dialog/Alert"

export const PlayerClearWageringRequirementDisclosure = ({
    playerCode,
    onSubmitted,
    ...prop
}) => {
    let dialog = useModalState()
    let label = useTrans({
        zh: "清除滿足提款金額",
        en: "Clear Wagering Requirement"
    })

    console.log(
        "PlayerClearWageringRequirementDisclosure playerCode",
        playerCode
    )
    return (
        <>
            <Disclosure {...dialog}>{label}</Disclosure>
            {dialog.visible && (
                <ResetDialog
                    playerCode={playerCode}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const ResetDialog = ({ playerCode, username, onSubmitted, dialog }) => {
    let title = useTrans({
        zh: "清除滿足提款金額？",
        en: "Clear Wagering Requirement？"
    })
    let [formData, setSubmit] = useState()
    let { isLoading, result, error, mutate, deleteCache } =
        useClearPlayerWageringRequirement(formData)

    const onSubmit = () => {
        setSubmit(playerCode)
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <>
            <Backdrop {...dialog}>
                <WarningConfirmFormDialog
                    onSubmit={onSubmit}
                    stopPropagation={true}
                    onSubmitted={onSubmitted}
                    isLoading={isLoading}
                    result={result}
                    error={error}
                    dialog={dialog}
                    hideBackdrop={true}
                    title={title}
                ></WarningConfirmFormDialog>
            </Backdrop>
            {error && <AlertDialog {...error} onClickClose={reset} />}
        </>
    )
}
