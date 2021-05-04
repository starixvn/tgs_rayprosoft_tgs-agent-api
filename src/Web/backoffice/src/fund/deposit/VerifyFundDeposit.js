import React, { useState, useEffect, useMemo } from "react"
import { Spinner, useModalState, FieldStack, Box, Text, Button } from "bumbag"
import { getTrans, Trans, useLocale, useTrans } from "../../i18n"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update } from "./menu"
import { Disclosure, VerifyDisclosure } from "../../dialog/Disclosure"
import { FormDialog } from "../../form/FormDialog"
import { useFundDepositForm, useVerifyFundDepositForm } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import clone from "clone-deep"
import { useForm, useFormContext } from "react-hook-form"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import { FormBody } from "./ViewFundDeposit"
import ReviewFund from "../ReviewFund"

export const VerifyFundDepositDisclosure = ({
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
                <VerifyFundDepositDialog
                    brandId={brandId}
                    formId={formId}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const VerifyFundDepositDialog = ({
    brandId,
    formId,
    onSubmitted,
    dialog,
    ...props
}) => {
    let title = useTrans({
        zh: "充值审核",
        en: "Review Deposit"
    })

    let approveLabel = useTrans({
        zh: "通过",
        en: "Approve"
    })

    let formMethods = useForm()
    let { setValue } = formMethods

    let {
        isLoading: isLoadingFundDepositForm,
        data: fundDepositForm,
        error: fundDepositFormError
    } = useFundDepositForm(brandId, formId)

    let [formData, setSubmit] = useState()
    let {
        isLoading,
        result,
        error,
        mutate,
        deleteCache
    } = useVerifyFundDepositForm(brandId, formId, formData)

    if (isLoadingFundDepositForm) {
        return <LoadingBackdrop {...dialog} />
    }

    if (fundDepositFormError) {
        return <AlertDialog {...fundDepositFormError} {...dialog} />
    }

    const onSubmit = ({ remark, reviewResponse }) => {
        setSubmit({
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
            defaultValues={clone(fundDepositForm)}
            actionButtonsProps={{
                submitText: approveLabel,
                palette: "success"
            }}
            onClickSubmit={(event) => {
                setValue("reviewResponse", ReviewFund.Approve)
            }}
            addonButtons={
                <Button
                    type="button"
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
        >
            <FormBody record={fundDepositForm} />
        </FormDialog>
    )
}
