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
import { Dialog, Button, Box } from "bumbag"
import { CancelText, SubmitText } from "../button/ActionButtons"
import AlertDialog from "../dialog/Alert"

export const FormDialog = ({
    defaultValues,
    onSubmit,
    onClickSubmit,
    onClickCancel,
    isLoading,
    result,
    error,
    onCloseErrorAlert,
    onSubmitted,
    children,
    dialog,
    hideOnClickOutside = false,
    showCloseButton = true,
    showActionButtons = true,
    cancelText = <CancelText />,
    submitText = <SubmitText />,
    addonButtons,
    actionButtonsProps = {},
    formMethods,
    stopPropagation = false,
    ...props
}) => {
    let methods = useForm({ defaultValues })
    formMethods = formMethods ?? methods

    useEffect(() => {
        if (result && !error) {
            onSubmitted?.()
            dialog.hide()
            formMethods.reset()
        }
    }, [result])

    return (
        <>
            <Dialog.Modal
                wrap={(children) => (
                    <FormProvider {...formMethods}>
                        <form
                            onSubmit={(event) => {
                                if (stopPropagation) {
                                    event.stopPropagation()
                                }
                                formMethods.handleSubmit(onSubmit)(event)
                            }}
                            css={css`
                                height: 100%;
                                display: flex;
                                flex-direction: column;
                                justify-content: space-between;
                            `}
                        >
                            {children}
                        </form>
                    </FormProvider>
                )}
                showActionButtons={showActionButtons}
                actionButtonsProps={{
                    isLoading: isLoading,
                    onClickSubmit,
                    onClickCancel,
                    cancelText,
                    submitText,
                    addonButtons:
                        typeof addonButtons === "function"
                            ? addonButtons(formMethods)
                            : addonButtons,
                    ...actionButtonsProps
                }}
                showCloseButton={showCloseButton}
                hideOnClickOutside={hideOnClickOutside}
                {...dialog}
                {...props}
            >
                {children}
            </Dialog.Modal>
            {error && (
                <AlertDialog {...error} onClickClose={onCloseErrorAlert} />
            )}
        </>
    )
}

export const DeleteConfirmFormDialog = ({
    submitText = <ConfirmText />,
    ...props
}) => <FormDialog submitText={submitText} {...props} />

export default FormDialog
