import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Spinner, Stack, Flex, Box, Card, Set } from "bumbag"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import dateTimeColumn from "../../table/column/dateTime"
import { useAgentBalanceChanges } from "./client"
import { Trans, useTrans } from "../../i18n"
import { pastWeek } from "../../date/range"
import DateTimeRangeInput from "../../date/DateTimeRangeInput"
import SearchButton from "../../button/SearchButton"
import { Tab as _Tab, TabPanel } from "../../tab"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import isSameDate from "date-fns/isEqual"
import textColumn from "../../table/column/textColumn"
import { Descending } from "../../table/order"
import subHours from "date-fns/subHours"
import endOfToday from "date-fns/endOfToday"
import { numberDecimalColumn } from "../../table/column/numberColumn"
import { Select } from "../../select/Select"
import AlertDialog from "../../dialog/Alert"

export const Key = "BalanceChange"

export const AgentBalanceChangeTab = ({ ...props }) => {
    return (
        <_Tab tabId={Key} {...props}>
            <Trans zh="账变记录" en="Balance Change Records" />
        </_Tab>
    )
}

export const AgentBalanceChangePanel = ({ brandId, agentId, ...props }) => {
    let { selectedId } = props
    let isActive = selectedId === Key

    let dateRange = newDateRange()
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        accountChangeType: "",
        from: dateRange.start,
        to: dateRange.end,
        brandId,
        agentId
    })

    let { pageSize, accountChangeType, from, to } = state
    let { isLoading, data, error, mutate, deleteCache } =
        useAgentBalanceChanges(
            isActive
                ? toApiParams({
                      ...state,
                      sortby: "recordedOn",
                      orderby: Descending
                  })
                : undefined
        )

    let columns = useColumns({ items: data?.items })

    return (
        <TabPanel tabId={Key} {...props}>
            <Stack spacing="major-2">
                <SearchBar
                    accountChangeType={accountChangeType}
                    start={from}
                    end={to}
                    search={({ accountChangeType, start, end }) => {
                        setState({
                            pageIndex: 0,
                            accountChangeType,
                            from: start,
                            to: end
                        })
                        mutate()
                    }}
                    isLoading={isLoading}
                />
                <Table
                    columns={columns}
                    data={data?.items}
                    pageCount={pageCount(pageSize, data?.totalCount)}
                    setPageIndex={setPageIndex}
                    setSortOrder={setSortOrder}
                    initialState={state}
                    isLoading={isLoading}
                />
            </Stack>
            {error && (
                <AlertDialog
                    {...error}
                    onClickClose={() => {
                        deleteCache()
                        mutate({ data: undefined }, false)
                    }}
                />
            )}
        </TabPanel>
    )
}

const SearchBar = ({
    accountChangeType,
    start,
    end,
    search,
    isLoading,
    ...props
}) => {
    let formMethods = useForm({
        defaultValues: {
            accountChangeType,
            start,
            end
        }
    })

    const onSubmit = (data) => search(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <AgentTransactionTypeSelect name="accountChangeType" />
                    <DateTimeRangeInput />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ items = [] }) =>
    useMemo(
        () => [
            dateTimeColumn({
                Header: <Trans zh="操作时间" en="Operation Time" />,
                accessor: "recordedOn",
                disableSortBy: true
            }),
            textColumn({
                accessor: "transactionType",
                Header: <Trans zh="交易类型" en="Transaction Type" />,
                disableSortBy: true,
                Cell: ({ value }) => <AgentTransactionTypeLabel value={value} />
            }),
            numberDecimalColumn({
                accessor: "amount",
                Header: <Trans zh="交易金额" en="Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "balance",
                Header: <Trans zh="余额" en="Balance" />,
                disableSortBy: true
            })
        ],
        [JSON.stringify(items)]
    )

const newDateRange = () => ({
    start: subHours(Date.now(), 1),
    end: endOfToday()
})

const Deposit = "Deposit"
const DepositTrans = {
    zh: "存款",
    en: "Deposit"
}

const Withdrawal = "Withdrawal"
const WithdrawalTrans = {
    zh: "提款",
    en: "Withdrawal"
}

const CancelWithdrawal = "CancelWithdrawal"
const CancelWithdrawalTrans = {
    zh: "取消提款",
    en: "Cancel Withdrawal"
}

const Adjust = "Adjust"
const AdjustTrans = {
    zh: "调账",
    en: "Balance Adjustment"
}

const Dividend = "Dividend"
const DividendTrans = {
    zh: "分紅",
    en: "Dividend"
}

const AgentTransactionTypeSelect = ({ placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "交易类型",
        en: "Transaction Type"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let options = [
        {
            label: useTrans(DepositTrans),
            value: Deposit
        },
        {
            label: useTrans(WithdrawalTrans),
            value: Withdrawal
        },
        {
            label: useTrans(CancelWithdrawalTrans),
            value: CancelWithdrawal
        },
        {
            label: useTrans(AdjustTrans),
            value: Adjust
        },
        {
            label: useTrans(DividendTrans),
            value: Dividend
        }
    ]

    return <Select placeholder={placeholder} options={options} {...props} />
}

const AgentTransactionTypeLabel = ({ value = "" }) => {
    switch (value) {
        case Deposit:
            return <Trans {...DepositTrans} />
        case Withdrawal:
            return <Trans {...WithdrawalTrans} />
        case Adjust:
            return <Trans {...AdjustTrans} />
        case Dividend:
            return <Trans {...DividendTrans} />
        case CancelWithdrawal:
            return <Trans {...CancelWithdrawalTrans} />
        default:
            return value
    }
}
