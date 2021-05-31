import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Heading, Stack, Button, Set, Text, Box, Level } from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute
} from "../../user/context"
import { Code } from "./menu"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"
import { SearchButton } from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { FormProvider, useForm } from "react-hook-form"
import { InputField } from "../../input/Input"
import textColumn from "../../table/column/textColumn"
import { useBrandPaymentList } from "../../system/brandpayments/client"
import { MerchantIdSelect } from "../../merchant/MerchantSelect"
import { BrandPaymentTransferTypeSelect } from "../../system/brandpayments/BrandPaymentTransferType"
import { PaymentProviderLabel } from "../../payment/PaymentProvider"
import { PaymentMethodLabel } from "../../payment/method/PaymentMethod"
import { PaymentChannelLabel } from "../../payment/channel/PaymentChannel"
import { BrandPaymentStatusSelect } from "../../system/brandpayments/BrandPaymentStatus"
import {
    AddBrandPaymentDisclosure,
    EditBrandPaymentDisclosure
} from "./BrandPayment"
import AlertDialog from "../../dialog/Alert"
import { PaymentMethodChannelListDisclosure } from "./PaymentMethodChannelList"
import { SortOrder } from "../../table/order"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = () => {
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        brandId: "",
        transferType: "",
        keyword: ""
    })

    let { isLoading, data, error, mutate, deleteCache } = useBrandPaymentList({
        ...toApiParams(state),
        sortby: "createdOn",
        orderby: SortOrder.Descending
    })

    let items = data?.items
    let columns = useColumns({ items, mutate })

    return (
        <PageContent
            title={<Trans zh="品牌支付管理" en="Brand Payment Management" />}
            actionBar={
                <Level>
                    <SearchBar
                        search={(newState) => {
                            setState({ pageIndex: 0, ...newState })
                            mutate()
                        }}
                        isLoading={isLoading}
                    />
                    <Set orientation="horizontal">
                        <PaymentMethodChannelListDisclosure
                            onSubmitted={mutate}
                        />
                        <AddBrandPaymentDisclosure
                            brandId={state.brandId}
                            onSubmitted={mutate}
                        />
                    </Set>
                </Level>
            }
        >
            <Table
                columns={columns}
                data={items}
                pageCount={pageCount(state.pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
            />
            {error && (
                <AlertDialog
                    {...error}
                    onClickClose={() => {
                        deleteCache()
                        mutate({ data: undefined }, false)
                    }}
                />
            )}
        </PageContent>
    )
}

const SearchBar = ({ search, isLoading, ...props }) => {
    let formMethods = useForm()
    const onSubmit = (data) => search?.(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <MerchantIdSelect required={false} />
                    <BrandPaymentTransferTypeSelect name="transferType" />
                    <BrandPaymentStatusSelect name="paymentStatus" />
                    <InputField
                        name="keyword"
                        showEmptyFieldValueAddonAfter
                        placeholder="Keyword"
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ items = [], mutate }) =>
    useMemo(
        () => [
            textColumn({
                accessor: "transferType",
                Header: <Trans zh="Transfer Type" en="Transfer Type" />,
                disableSortBy: true,
                Cell: ({ value }) => value
            }),
            textColumn({
                accessor: "paymentId",
                Header: <Trans zh="支付厂商" en="Payment Name" />,
                disableSortBy: true,
                Cell: ({ value }) => <PaymentProviderLabel value={value} />
            }),
            textColumn({
                accessor: "paymentMethodId",
                Header: <Trans zh="支付方式" en="Payment Method" />,
                disableSortBy: true,
                Cell: ({ value }) => <PaymentMethodLabel value={value} />
            }),
            textColumn({
                accessor: "paymentChannelId",
                Header: <Trans zh="支付通道" en="Payment Channel" />,
                disableSortBy: true,
                Cell: ({ value }) => <PaymentChannelLabel value={value} />
            }),
            textColumn({
                accessor: "channelName",
                Header: <Trans zh="通道名称" en="Channel Name" />,
                disableSortBy: true
            }),
            textColumn({
                id: "actions",
                accessor: (row) => row,
                Header: <Trans zh="Actions" en="Actions" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    return (
                        <EditBrandPaymentDisclosure
                            record={value}
                            onSubmitted={mutate}
                        />
                    )
                }
            })
        ],
        [JSON.stringify(items)]
    )
