import React, { useState, useEffect, useMemo } from "react"
import { Spinner, useModalState, FieldStack, Box, Text, Button } from "bumbag"
import { getTrans, Trans, useLocale, useTrans } from "../../i18n"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update } from "./menu"
import { Disclosure, VerifyDisclosure } from "../../dialog/Disclosure"
import { FormDialog } from "../../form/FormDialog"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import clone from "clone-deep"
import { useForm, useFormContext } from "react-hook-form"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import { FormBody } from "./ViewFundWithdraw"
import { useFundWithdrawForm, useVerifyFundWithdrawForm } from "./client"
import ReviewFund from "../ReviewFund"

export const VerifyFundWithdrawDisclosure = ({
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
            {dialog.visible && (
                <VerifyFundWithdrawDialog
                    brandId={brandId}
                    formId={formId}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const VerifyFundWithdrawDialog = ({
    brandId,
    formId,
    onSubmitted,
    dialog,
    ...props
}) => {
    let title = useTrans({
        zh: "提款审核",
        en: "Review Withdraw"
    })

    let approveLabel = useTrans({
        zh: "通过",
        en: "Approve"
    })

    let formMethods = useForm()
    let { setValue } = formMethods

    let {
        isLoading: isLoadingFundWithdrawForm,
        data: fundWithdrawForm,
        error: fundWithdrawFormError
    } = useFundWithdrawForm(brandId, formId)

    let [formData, setSubmit] = useState()
    let {
        isLoading,
        result,
        error,
        mutate,
        deleteCache
    } = useVerifyFundWithdrawForm(brandId, formId, formData)

    if (isLoadingFundWithdrawForm) {
        return <LoadingBackdrop {...dialog} />
    }

    if (fundWithdrawFormError) {
        return <AlertDialog {...fundWithdrawFormError} {...dialog} />
    }

    const onSubmit = ({ remark, reviewResponse, brandPaymentId, ...rest }) => {
        setSubmit({
            brandId,
            brandPaymentId,
            remark,
            reviewResponse
        })
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <FormDialog
            formMethods={formMethods}
            defaultValues={clone({ ...fundWithdrawForm })}
            actionButtonsProps={{
                submitText: approveLabel,
                palette: "success"
            }}
            onClickSubmit={(event) => {
                setValue("reviewResponse", ReviewFund.Approve)
            }}
            addonButtons={
                <Button
                    type="submit"
                    onClick={() => {
                        setValue("reviewResponse", ReviewFund.Reject)
                    }}
                    palette="danger"
                    variant="outlined"
                >
                    <Trans zh="拒绝" en="Reject" />
                </Button>
            }
            onSubmit={onSubmit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={onSubmitted}
            title={title}
            dialog={dialog}
            maxWidth="80vw"
        >
            <FormBody brandId={brandId} record={fundWithdrawForm} />
        </FormDialog>
    )
}
