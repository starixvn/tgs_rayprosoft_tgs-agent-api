import React, {
    useState,
    useEffect,
    useMemo,
    useRef,
    createContext,
    useContext
} from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import { Dialog, Button, Box, Set } from "bumbag"
import { CancelText, CloseText, SubmitText } from "../button/ActionButtons"
import AlertDialog from "../dialog/Alert"

export const ConfirmButtonFooter = ({
    isLoading,
    disabled,
    width = "100%",
    alignX = "center",
    confirmText: ConfirmText = SubmitText,
    ...props
}) => {
    return (
        <Box width={width} alignX={alignX} {...props}>
            <Button
                palette="primary"
                type="submit"
                isLoading={isLoading}
                disabled={disabled}
            >
                {typeof ConfirmText === "function" ? (
                    <ConfirmText />
                ) : (
                    ConfirmText
                )}
            </Button>
        </Box>
    )
}

export const ConfirmCancelFooter = ({
    isLoading,
    disabled,
    width = "100%",
    alignX = "center",
    confirmText: ConfirmText = SubmitText,
    cancelText: CancelTextLabel = CancelText,
    onClickClose,
    dialog,
    ...props
}) => {
    return (
        <Set orientation="horizontal" width={width} alignX={alignX} {...props}>
            <Button
                palette="primary"
                type="submit"
                isLoading={isLoading}
                disabled={disabled}
            >
                {typeof ConfirmText === "function" ? (
                    <ConfirmText />
                ) : (
                    ConfirmText
                )}
            </Button>
            <Button
                type="button"
                onClick={() => {
                    dialog?.hide()
                    onClickClose?.()
                }}
            >
                {typeof CancelTextLabel === "function" ? (
                    <CancelTextLabel />
                ) : (
                    CancelTextLabel
                )}
            </Button>
        </Set>
    )
}

export const CloseButtonFooter = ({
    width = "100%",
    alignX = "center",
    dialog,
    onClickClose,
    ...props
}) => {
    return (
        <Box width={width} alignX={alignX} {...props}>
            <Button
                palette="primary"
                type="button"
                onClick={() => {
                    dialog?.hide()
                    onClickClose?.()
                }}
            >
                <CloseText />
            </Button>
        </Box>
    )
}

export default ConfirmButtonFooter
