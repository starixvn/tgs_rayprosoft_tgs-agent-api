import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Spinner, Stack, Flex, Box, Card, Set } from "bumbag"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import dateTimeColumn from "../../table/column/dateTime"
import { usePlayerBalanceChanges } from "./client"
import { Trans, useTrans } from "../../i18n"
import DateTimeRangeInput from "../../date/DateTimeRangeInput"
import SearchButton from "../../button/SearchButton"
import { Tab as _Tab, TabPanel } from "../../tab"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import textColumn from "../../table/column/textColumn"
import { Descending } from "../../table/order"
import subHours from "date-fns/subHours"
import endOfToday from "date-fns/endOfToday"
import { numberDecimalColumn } from "../../table/column/numberColumn"
import { Select } from "../../select/Select"
import AlertDialog from "../../dialog/Alert"

export const Key = "BalanceChange"

export const PlayerBalanceChangeTab = ({ ...props }) => {
    return (
        <_Tab tabId={Key} {...props}>
            <Trans zh="账变记录" en="Balance Change Records" />
        </_Tab>
    )
}

export const PlayerBalanceChangePanel = ({ brandId, playerCode, ...props }) => {
    let { selectedId } = props
    let isActive = selectedId === Key

    let dateRange = newDateRange()
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        accountChangeType: "",
        from: dateRange.start,
        to: dateRange.end,
        brandId,
        playerCode
    })

    let { pageSize, accountChangeType, from, to } = state
    let { isLoading, data, error, mutate, deleteCache } =
        usePlayerBalanceChanges(
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
                    controlledState={state}
                    isLoading={isLoading}
                />
            </Stack>
            {error && <AlertDialog {...error} onClickClose={deleteCache} />}
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
                    <PlayerTransactionTypeSelect name="accountChangeType" />
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
                id: "transactionType",
                accessor: (row) => row,
                Header: <Trans zh="交易类型" en="Transaction Type" />,
                disableSortBy: true,
                Cell: ({ value: record }) => {
                    let { transactionType, gameActionType } = record
                    if (transactionType) {
                        return (
                            <PlayerTransactionTypeLabel
                                value={transactionType}
                            />
                        )
                    }
                    return <GameActionLabel value={gameActionType} />
                }
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
const Adjust = "Adjust"
const AdjustTrans = {
    zh: "调账",
    en: "Balance Adjustment"
}

const Bonus = "Bonus"
const BonusTrans = {
    zh: "红利",
    en: "Bonus"
}

const Rebate = "Rebate"
const RebateTrans = {
    zh: "返水",
    en: "Commission Rebate"
}

const Dividend = "Dividend"
const DividendTrans = {
    zh: "分紅",
    en: "Profit Sharing"
}

const Promotion = "Promotion"
const PromotionTrans = {
    zh: "活动",
    en: "Promotion"
}

const GameProviderFundIn = "GameProviderFundIn"
const GameProviderFundInTrans = {
    zh: "转入",
    en: "Fund In"
}

const GameProviderFundOut = "GameProviderFundOut"
const GameProviderFundOutTrans = {
    zh: "转出",
    en: "Fund Out"
}

const CancelWithdrawal = "CancelWithdrawal"
const CancelWithdrawalTrans = {
    zh: "取消提款",
    en: "Cancel Withdrawal"
}

const PlayerTransactionTypeSelect = ({ placeholder, ...props }) => {
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
            label: useTrans(BonusTrans),
            value: Bonus
        },
        {
            label: useTrans(RebateTrans),
            value: Rebate
        },
        {
            label: useTrans(DividendTrans),
            value: Dividend
        },
        {
            label: useTrans(PromotionTrans),
            value: Promotion
        },
        {
            label: useTrans(GameProviderFundOutTrans),
            value: GameProviderFundOut
        },
        {
            label: useTrans(GameProviderFundInTrans),
            value: GameProviderFundIn
        },
        {
            label: useTrans(PlaceBetTrans),
            value: PlaceBet
        },
        {
            label: useTrans(WinBetTrans),
            value: WinBet
        },
        {
            label: useTrans(LoseBetTrans),
            value: LoseBet
        },
        {
            label: useTrans(FreeBetTrans),
            value: FreeBet
        },
        {
            label: useTrans(TieBetTrans),
            value: TieBet
        },
        {
            label: useTrans(CancelTransactionTrans),
            value: CancelTransaction
        },
        {
            label: useTrans(LockAmountTrans),
            value: LockAmount
        },
        {
            label: useTrans(UnlockAmountTrans),
            value: UnlockAmount
        },
        {
            label: useTrans(WinJackpotTrans),
            value: WinJackpot
        }
    ]

    return <Select placeholder={placeholder} options={options} {...props} />
}

const PlayerTransactionTypeLabel = ({ value = "" }) => {
    switch (value) {
        case Deposit:
            return <Trans {...DepositTrans} />
        case Withdrawal:
            return <Trans {...WithdrawalTrans} />
        case Adjust:
            return <Trans {...AdjustTrans} />
        case Bonus:
            return <Trans {...BonusTrans} />
        case Rebate:
            return <Trans {...RebateTrans} />
        case Dividend:
            return <Trans {...DividendTrans} />
        case Promotion:
            return <Trans {...PromotionTrans} />
        case GameProviderFundOut:
            return <Trans {...GameProviderFundOutTrans} />
        case GameProviderFundIn:
            return <Trans {...GameProviderFundInTrans} />
        case CancelWithdrawal:
            return <Trans {...CancelWithdrawalTrans} />
        default:
            return value
    }
}

const PlaceBet = "PlaceBet"
const PlaceBetTrans = {
    zh: "投注",
    en: "Place Bet"
}

const WinBet = "WinBet"
const WinBetTrans = {
    zh: "赢",
    en: "Win Bet"
}

const LoseBet = "LoseBet"
const LoseBetTrans = {
    zh: "输",
    en: "Lose Bet"
}

const FreeBet = "FreeBet"
const FreeBetTrans = {
    zh: "免费投注",
    en: "FreeB et"
}

const TieBet = "TieBet"
const TieBetTrans = {
    zh: "和局",
    en: "Tie Bet"
}

const CancelTransaction = "CancelTransaction"
const CancelTransactionTrans = {
    zh: "取消投注",
    en: "Cancel Transaction"
}
const LockAmount = "LockAmount"
const LockAmountTrans = {
    zh: "锁定金额",
    en: "Lock Amount"
}

const UnlockAmount = "UnlockAmount"
const UnlockAmountTrans = {
    zh: "解锁金额",
    en: "Unlock Amount"
}

const WinJackpot = "WinJackpot"
const WinJackpotTrans = {
    zh: "赢 Jackpot",
    en: "Win Jackpot"
}

const GameActionLabel = ({ value = "" }) => {
    switch (value) {
        case PlaceBet:
            return <Trans {...PlaceBetTrans} />
        case WinBet:
            return <Trans {...WinBetTrans} />
        case LoseBet:
            return <Trans {...LoseBetTrans} />
        case FreeBet:
            return <Trans {...FreeBetTrans} />
        case TieBet:
            return <Trans {...TieBetTrans} />
        case CancelTransaction:
            return <Trans {...CancelTransactionTrans} />
        case LockAmount:
            return <Trans {...LockAmountTrans} />
        case UnlockAmount:
            return <Trans {...UnlockAmountTrans} />
        case WinJackpot:
            return <Trans {...WinJackpotTrans} />
        default:
            return value
    }
}
