import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
  useModalState,
  Stack,
  Heading,
  Box,
  Text,
  FieldStack,
  Button
} from "bumbag"
import { useIsDisplayMenuByRoute, useUserLicenseeId } from "../../user/context"
import Disclosure, { AddDisclosure } from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Update as UpdateMenu } from "./menu"
import { Trans, useTrans } from "../../i18n"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import {
  CloseButtonFooter,
  ConfirmButtonFooter
} from "../../form/ConfirmButtonFooter"
import { Input, InputField } from "../../input/Input"
import { MerchantIdSelect } from "../MerchantSelect"
import { RoleTypes } from "../../system/roles/type"
import { useAddBrand, useBrand, useUpdateBrand } from "./client"
import { useFormContext, useFieldArray, useForm } from "react-hook-form"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import Textarea from "../../textarea"
import { BrandLanguageCheckboxGroupTable } from "./BrandLanguageCheckboxGroup"
import { BrandGameProviderCheckboxGroupTable } from "./BrandGameProviderCheckboxGroup"
import { BrandBetLimitCheckboxGroupTable } from "./BrandBetLimitCheckboxGroup"
import useLicenseeBrand from "./useLicenseeBrand"
import CurrencyFieldsBox, {
  mapCheckedCurrencyCodes,
  toCheckedCurrencyCodes
} from "../CurrencyFieldsBox"
import { toCheckedFieldName } from "../../currency/CurrencyCheckboxGroup"

export const AddButton = ({ onSubmitted, ...props }) => {
  let dialog = useModalState()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <AddDisclosure {...dialog} />
      {dialog.visible && (
        <AddDialog onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

const AddDialog = ({ onSubmitted, dialog }) => {
  let userLicenseeId = useUserLicenseeId()
  let formMethods = useForm({
    defaultValues: {
      licenseeId: userLicenseeId
    }
  })
  let { watch } = formMethods
  let resellerId = watch("resellerId")
  let licenseeId = watch("licenseeId")
  let {
    licensee,
    languages,
    currencies,
    gameProviders,
    betLimits,
    isLoading: isLoadingLicensee,
    error: licenseeError
  } = useLicenseeBrand({ licenseeId })

  useEffect(() => {
    if (licensee) {
      formMethods.reset({
        resellerId,
        licenseeId,
        languages,
        currencies,
        gameProviders,
        betLimits
      })
    }
  }, [licensee])

  let [formData, setSubmit] = useState()
  let { result, isLoading, error, mutate } = useAddBrand(formData)

  const onSubmit = (data) => {
    let formData = normalizeFormData(data)
    let gameProviders =
      formData.gameProviders.map(
        ({ checked, id, ...gameProvider }) => gameProvider
      ) ?? []
    setSubmit({
      ...formData,
      gameProviders
    })
  }

  const reset = () => {
    mutate(undefined, false)
    setSubmit(undefined)
  }

  let title = useTrans({
    zh: "新增品牌",
    en: "Add Brand"
  })
  return (
    <FormDialog
      formMethods={formMethods}
      onSubmit={onSubmit}
      result={result}
      isLoading={isLoading}
      error={error}
      onCloseErrorAlert={reset}
      onSubmitted={() => {
        reset()
        onSubmitted?.()
      }}
      showActionButtons={false}
      footer={
        <ConfirmButtonFooter isLoading={isLoading} disabled={isLoading} />
      }
      title={title}
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
      <EditBody isAdd={true} />
    </FormDialog>
  )
}

export const ViewButton = ({ record, ...props }) => {
  let dialog = useModalState()

  /* TODO: view auth?
  let isDisplay = useIsDisplayMenuByRoute([ViewMenu])
  if (!isDisplay) {
    return null
  }
  */
  return (
    <>
      <Disclosure {...dialog} {...props}>
        <Trans zh="查看" en="View" />
      </Disclosure>
      {dialog.visible && (
        <ViewDialog
          brandId={record?.id}
          licenseeId={record?.licenseeId}
          dialog={dialog}
        />
      )}
    </>
  )
}

const ViewDialog = ({ brandId, licenseeId, dialog, ...props }) => {
  let {
    brand,
    licensee,
    languages,
    currencies,
    gameProviders,
    games,
    betLimits,
    isLoading,
    error,
    mutateBrand,
    deleteBrandCache
  } = useLicenseeBrand({ brandId, licenseeId })

  let title = useTrans({
    zh: "查看品牌",
    en: "View Brand"
  })

  if (isLoading) {
    return <LoadingBackdrop {...dialog} onClickClose={dialog.hide} />
  }

  if (error) {
    return <AlertDialog {...error} {...dialog} onClickClose={dialog.hide} />
  }

  const reset = () => {
    mutateBrand(undefined, false)
    deleteBrandCache()
  }

  let record = {
    ...brand,
    licensee,
    languages,
    currencies,
    gameProviders,
    games,
    betLimits
  }
  return (
    <FormDialog
      defaultValues={record}
      onClickClose={reset}
      title={title}
      dialog={dialog}
      showActionButtons={false}
      onClickClose={reset}
      footer={<CloseButtonFooter dialog={dialog} onClickClose={reset} />}
      placement="top"
      minWidth="60vw"
      maxWidth="unset"
      css={css`
        & .bb-DialogContent {
          position: relative;
        }
      `}
    >
      <EditBody readonly record={record} />
    </FormDialog>
  )
}

export const EditButton = ({ record, onSubmitted, ...props }) => {
  let dialog = useModalState()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="修改" en="Edit" />
      </Disclosure>
      {dialog.visible && (
        <EditDialog
          brandId={record?.id}
          licenseeId={record?.licenseeId}
          onSubmitted={onSubmitted}
          dialog={dialog}
        />
      )}
    </>
  )
}

const EditDialog = ({ brandId, licenseeId, onSubmitted, dialog, ...props }) => {
  let {
    brand,
    licensee,
    languages,
    currencies,
    gameProviders,
    games,
    betLimits,
    isLoading: isLoadingBrand,
    error: brandError,
    mutateBrand,
    deleteBrandCache
  } = useLicenseeBrand({ brandId, licenseeId })

  let [formData, setSubmit] = useState()
  let { result, isLoading, error, mutate } = useUpdateBrand(brandId, formData)

  let title = useTrans({
    zh: "编辑品牌",
    en: "Edit Brand"
  })

  if (isLoadingBrand) {
    return <LoadingBackdrop {...dialog} />
  }

  if (brandError) {
    return (
      <AlertDialog {...brandError} {...dialog} onClickClose={dialog.hide} />
    )
  }

  const onSubmit = (data) => {
    let formData = {
      ...brand,
      ...normalizeFormData(data)
    }
    setSubmit(formData)
  }

  const reset = () => {
    mutateBrand(undefined, false)
    deleteBrandCache()

    mutate(undefined, false)
    setSubmit(undefined)
  }

  let record = {
    ...brand,
    licensee,
    languages,
    currencies,
    gameProviders,
    games,
    betLimits
  }

  return (
    <FormDialog
      defaultValues={record}
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
        <ConfirmButtonFooter isLoading={isLoading} disabled={isLoading} />
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
      <EditBody record={record} />
    </FormDialog>
  )
}

const normalizeFormData = ({
  languages,
  currencies,
  gameProviders,
  betLimits,
  ...data
}) => {
  languages = languages.filter(({ checked }) => checked)

  let currencyCodes = toCheckedCurrencyCodes(currencies)

  betLimits = betLimits
    .filter((betLimit) => betLimit.checked)
    .map(({ id, ...props }) => ({ id: parseInt(id), ...props }))

  return {
    ...data,
    languages,
    currencyCodes,
    gameProviders,
    betLimits
  }
}

const EditBody = ({ record, readonly, isAdd = false }) => {
  let { watch, getValues, register, control } = useFormContext()

  let userLicenseeId = useUserLicenseeId()

  let licensee = watch("licensee")
  let editable = !readonly

  let apiKeyInputRef = useRef()
  let currencyFieldArrayName = "currencies"
  return (
    <Stack orientation="vertical">
      <Heading use="h5" marginBottom="major-1">
        <Trans zh="基本资料" en="Basic Information" />
      </Heading>

      <FieldValues>
        {!userLicenseeId && (
          <>
            <NormalLabel>
              <Trans zh="所属公司" en="Licensee" />
            </NormalLabel>
            <Box alignY="center">
              {isAdd && <MerchantIdSelect end={RoleTypes.Licensee} />}
              {!isAdd && <Text.Block>{licensee?.name}</Text.Block>}
            </Box>
          </>
        )}

        <NormalLabel>
          <Trans zh="品牌代码" en="Code" />
        </NormalLabel>
        <Box alignY="center">
          <Input name="code" type={isAdd ? "input" : "hidden"} />
          {!isAdd && <Text.Block>{getValues("code")}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="品牌名称" en="Name" />
        </NormalLabel>
        <Box alignY="center">
          {editable && <Input name="name" required />}
          {readonly && <Text.Block>{record?.name}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="品牌简介" en="Description" />
        </NormalLabel>
        <Box alignY="center">
          {editable && <Textarea name="description" />}
          {readonly && <Text.Block>{record.description}</Text.Block>}
        </Box>

        <NormalLabel>
          <Trans zh="品牌邮箱" en="Email" />
        </NormalLabel>
        <Box alignY="center">
          {editable && <Input type="email" name="operationalEmail" required />}
          {readonly && <Text.Block>{record.operationalEmail}</Text.Block>}
        </Box>

        {!isAdd && (
          <>
            <NormalLabel>
              <Trans zh="API密钥 " en="API Key" />
            </NormalLabel>
            <Box alignY="center">
              <Input type="hidden" name="apiKey" />
              <InputField
                readonly
                inputRef={apiKeyInputRef}
                defaultValue={getValues("apiKey")}
                addonAfter={
                  <Button
                    onClick={() => {
                      let target = apiKeyInputRef.current
                      target.select()
                      document.execCommand("copy")
                    }}
                  >
                    <Trans zh="复制" en="Copy" />
                  </Button>
                }
                css={css`
                  & input {
                    text-overflow: ellipsis;
                  }
                `}
              />
            </Box>
          </>
        )}

        <LanguageFieldsBox readonly={readonly} />

        <CurrencyFieldsBox readonly={readonly} />

        <BrandBetLimitFieldsBox
          readonly={readonly}
          currencyFieldArrayName={currencyFieldArrayName}
        />

        <BrandGameProviderFieldsBox readonly={readonly} />
      </FieldValues>
    </Stack>
  )
}

const LanguageFieldsBox = ({ readonly }) => {
  let { watch, control } = useFormContext()

  let fieldsName = "languages"
  let fieldArrayContext = useFieldArray({
    control,
    name: fieldsName,
    keyName: "_id"
  })
  let languages = watch(fieldsName)

  if (languages.length == 0) {
    return null
  }

  return (
    <>
      <NormalLabel>
        <Trans zh="语言" en="Language" />
      </NormalLabel>
      <BrandLanguageCheckboxGroupTable
        fieldArrayContext={fieldArrayContext}
        disabled={readonly}
        showBrandCultureCode
      />
    </>
  )
}

const BrandGameProviderFieldsBox = ({ readonly }) => {
  let { watch, control } = useFormContext()

  let fieldsName = "gameProviders"
  let fieldArrayContext = useFieldArray({
    control,
    name: fieldsName,
    keyName: "_id"
  })
  let gameProviders = watch(fieldsName)

  if (gameProviders.length == 0) {
    return null
  }

  return (
    <>
      <NormalLabel>
        <Trans zh="游戏厂商" en="Game Provider" />
      </NormalLabel>
      <BrandGameProviderCheckboxGroupTable
        marginTop="major-1"
        fieldArrayContext={fieldArrayContext}
        disabled={readonly}
      />
    </>
  )
}

const BrandBetLimitFieldsBox = ({ currencyFieldArrayName, readonly }) => {
  let { watch, control, getValues } = useFormContext()

  let fieldsName = "betLimits"
  let fieldArrayContext = useFieldArray({
    control,
    name: fieldsName,
    keyName: "_id"
  })

  let currencies = watch(currencyFieldArrayName)
  let betLimits = watch(fieldsName)

  if (betLimits.length == 0) {
    return null
  }

  let requiredCurrencyCodes = mapCheckedCurrencyCodes(
    currencyFieldArrayName,
    toCheckedFieldName,
    getValues,
    currencies
  )

  return (
    <>
      <NormalLabel>
        <Trans zh="品牌限红" en="Brand Bet Limit" />
      </NormalLabel>
      <FieldStack marginTop="major-1">
        <BrandBetLimitCheckboxGroupTable
          fieldArrayContext={fieldArrayContext}
          disabled={readonly}
          requiredCurrencyCodes={requiredCurrencyCodes}
        />
      </FieldStack>
    </>
  )
}
