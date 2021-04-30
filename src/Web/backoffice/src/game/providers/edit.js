import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext } from "react-hook-form"
import {
    Stack,
    Heading,
    Box,
    Button,
    Text,
    Group,
    Set,
    Flex,
    useModalState
} from "bumbag"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update as UpdateMenu } from "../providers/menu"
import { EditDisclosure } from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Trans, useTrans } from "../../i18n"
import { useGameProvider, useUpdateGameProvider } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import { Input } from "../../input/Input"
import { Textarea } from "../../textarea"
import { GameProviderStatusSelect } from "./status"
import { useLanguageList } from "../../language/client"
import { useCurrencyList } from "../../currency/client"
import { GameProviderCurrencyCheckboxGroupTable } from "./GameProviderCurrencyCheckboxGroup"
import { GameProviderLanguageCheckboxGroupTable } from "./GameProviderLanguageCheckboxGroup"
import {
    mapCheckedByCode,
    mapCheckedByCultureCode
} from "../../checkbox/mapChecked"
import ConfirmButtonFooter from "../../form/ConfirmButtonFooter"

export const EditButton = ({ record, onSubmitted, ...props }) => {
    let dialog = useModalState()
    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <EditDisclosure {...dialog} />
            {dialog.visible && (
                <EditDialog
                    id={record?.id}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const EditDialog = ({ id, dialog, ...props }) => {
    let {
        gameProvider,
        languages,
        currencies,
        isLoading: isLoadingFormData,
        error: formDataError,
        mutateGameProvider,
        deleteGameProviderCache
    } = useFormDataInit(id)
    let [formData, setSubmit] = useState()
    let { result, isLoading, error, mutate } = useUpdateGameProvider(
        id,
        formData
    )

    let title = useTrans({
        zh: "编辑游戏商",
        en: "Edit Game Provider"
    })

    if (isLoadingFormData) {
        return <LoadingBackdrop {...dialog} />
    }

    if (formDataError) {
        return <AlertDialog {...formDataError} onClickClose={dialog.hide} />
    }

    const onSubmit = (data) => {
        setSubmit(data)
    }

    const reset = () => {
        mutateGameProvider(undefined, false)
        deleteGameProviderCache()

        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <FormDialog
            defaultValues={{ ...gameProvider, languages, currencies }}
            onSubmit={onSubmit}
            result={result}
            isLoading={isLoading}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={() => {
                reset()
                onSubmitted?.()
            }}
            onClickClose={reset}
            title={title}
            showActionButtons={false}
            footer={
                <ConfirmButtonFooter
                    isLoading={isLoading}
                    disabled={isLoading}
                />
            }
            dialog={dialog}
            placement="top"
            minWidth="60vw"
            maxWidth="unset"
            css={css`
                & .bb-DialogContent {
                    position: relative;
                }
            `}
        >
            <EditBody />
        </FormDialog>
    )
}

const normalizeFormData = ({
    gameProviderCurrencies,
    gameProviderLanguages,
    ...data
}) => {
    let currencies = Object.entries(gameProviderCurrencies).reduce(
        (list, [key, { checked, ...props }]) => {
            if (checked) {
                return [...list, { code: key, ...props }]
            }

            return list
        },
        []
    )
    let languages = Object.entries(gameProviderLanguages).reduce(
        (list, [key, { checked, ...props }]) => {
            if (checked) {
                return [...list, { cultureCode: key, ...props }]
            }

            return list
        },
        []
    )
    return {
        currencies,
        languages,
        ...data
    }
}

const EditBody = ({ readonly, isAdd = false }) => {
    let { watch, getValues, register } = useFormContext()

    let editable = !readonly
    return (
        <Stack orientation="vertical">
            <Heading use="h5" marginBottom="major-1">
                <Trans zh="基本资料" en="Basic Information" />
            </Heading>

            <FieldValues>
                <NormalLabel>
                    <Trans zh="游戏商代码" en="Code" />
                </NormalLabel>
                <Box alignY="center">
                    <Input name="code" type={isAdd ? "input" : "hidden"} />
                    {!isAdd && <Text.Block>{getValues("code")}</Text.Block>}
                </Box>

                <NormalLabel>
                    <Trans zh="游戏商名称" en="Name" />
                </NormalLabel>
                <Box alignY="center">
                    {editable && <Input name="name" required />}
                    {readonly && <Text.Block>{getValues("name")}</Text.Block>}
                </Box>

                {/* TODO: no this field
        <NormalLabel>
          <Trans zh="游戏商简称" en="Short Name" />
        </NormalLabel>
        <Input name="shortName" required /> */}

                <NormalLabel>
                    <Trans zh="游戏商简介" en="Description" />
                </NormalLabel>
                <Input name="description" />

                <NormalLabel>
                    <Trans zh="状态" en="Status" />
                </NormalLabel>
                <GameProviderStatusSelect />

                <NormalLabel>
                    <Trans zh="支持币种" en="Currencies" />
                </NormalLabel>
                <GameProviderCurrencyCheckboxGroupTable disabled={readonly} />

                <NormalLabel>
                    <Trans zh="支持语言" en="Languages" />
                </NormalLabel>
                <GameProviderLanguageCheckboxGroupTable disabled={readonly} />
            </FieldValues>
        </Stack>
    )
}

const useFormDataInit = (gameProviderId) => {
    let {
        data: gameProvider,
        isLoading: isLoadingGameProvider,
        error: gameProviderError,
        mutate: mutateGameProvider,
        deleteCache: deleteGameProviderCache
    } = useGameProvider(gameProviderId)
    let {
        data: languageData,
        isLoading: isLoadingLanguage,
        error: languageError
    } = useLanguageList({ showAll: true })
    let {
        data: currencyData,
        isLoading: isLoadingCurrency,
        error: currencyError
    } = useCurrencyList({ showAll: true })

    return {
        gameProvider,
        languages: mapCheckedByCultureCode(
            gameProvider?.languages ?? [],
            languageData?.items ?? []
        ),
        currencies: mapCheckedByCode(
            gameProvider?.currencies ?? [],
            currencyData?.items ?? []
        ),
        isLoading:
            isLoadingGameProvider || isLoadingLanguage || isLoadingCurrency,
        error: gameProviderError || languageError || currencyError,
        mutateGameProvider,
        deleteGameProviderCache
    }
}
