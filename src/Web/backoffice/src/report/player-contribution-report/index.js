import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import {
    Heading,
    Stack,
    Box,
    Button,
    Set,
    useModalState,
    Text,
    Link
} from "bumbag"
import PageContent from "../../page/Content"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserBrandId
} from "../../user/context"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { FormattedNumber, Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { InputField } from "../../input/Input"
import { Code } from "./menu"
import BrandBar from "../BrandBar"
import { DateInputRange } from "../../date/DateTimeRangeInput"
import { yesterday } from "../../date/range"
import dateTimeColumn from "../../table/column/dateTime"
import subDays from "date-fns/subDays"
import { startOfNextDay } from "../../date/startOfNextDay"
import numberColumn, {
    numberDecimalColumn
} from "../../table/column/numberColumn"
import startOfToday from "date-fns/startOfToday"
import AlertDialog from "../../dialog/Alert"
import textColumn from "../../table/column/textColumn"
import VipLevelSelect from "../../vip-levels/VipLevelSelect"
import { usePlayerContributionList } from "./client"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

export const Body = () => {
    let userBrandId = useUserBrandId()
    let [brandId, setBrandId] = useState(userBrandId)

    return (
        <Set orientation="vertical" isFilled>
            <BrandBar
                onBrandChange={(value) => {
                    setBrandId(value)
                }}
            />
            {brandId && <ReportContent brandId={brandId} />}
        </Set>
    )
}

const ReportContent = ({ brandId }) => {
    let dateRange = yesterday()
    let QueryMaxRangeDays = 30
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        brandId,
        pageSize: QueryMaxRangeDays,
        username: undefined,
        level: undefined,
        from: dateRange.start,
        to: dateRange.end
    })

    useEffect(() => {
        setState({ pageIndex: 0, brandId })
    }, [brandId])

    let { pageIndex, pageSize, from, to } = state
    let {
        isLoading,
        data = {},
        error,
        mutate,
        deleteCache
    } = usePlayerContributionList({
        ...toApiParams(state),
        /*
        sortby: "username",
        */
        from,
        to: startOfNextDay(to)
    })

    let DataRetentionDays = 180
    let minDate = subDays(dateRange.start, DataRetentionDays)

    let columns = useColumns({
        brandId,
        start: from,
        end: to,
        items: data?.items
    })
    let maxDate = startOfToday()

    return (
        <PageContent
            actionBar={
                <SearchBar
                    brandId={brandId}
                    username={state?.username}
                    level={state?.level}
                    start={from}
                    end={to}
                    minDate={minDate}
                    maxDate={maxDate}
                    maxRangeDays={QueryMaxRangeDays}
                    isLoading={isLoading}
                    search={({ username, level, start, end }) => {
                        let newState = {
                            pageIndex: 0,
                            username,
                            level,
                            from: start,
                            to: end
                        }

                        if (
                            equals(newState, {
                                pageIndex,
                                username: state?.username,
                                level: state?.level,
                                from: state?.from,
                                to: state?.to
                            })
                        ) {
                            mutate()
                        }
                        setState(newState)
                    }}
                />
            }
            title={<Trans zh="玩家贡献报表" en="Player Contribution Report" />}
        >
            <Table
                controlledState={state}
                columns={columns}
                data={data?.items}
                pageCount={pageCount(pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
                foot={data?.items?.length > 0}
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

const SearchBar = ({
    brandId,
    username,
    level,
    start,
    end,
    minDate,
    maxDate,
    maxRangeDays,
    search,
    isLoading,
    ...props
}) => {
    let formMethods = useForm({
        defaultValues: {
            username,
            level,
            start,
            end
        }
    })

    let usernamePlaceholder = useTrans({
        zh: "玩家用户名",
        en: "Player username"
    })

    const onSubmit = (data) => search?.(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <InputField
                        name="username"
                        showEmptyFieldValueAddonAfter
                        placeholder={usernamePlaceholder}
                    />
                    <VipLevelSelect
                        name="level"
                        brandId={brandId}
                        optionValue={({ level }) => level}
                    />
                    <DateInputRange
                        minDate={minDate}
                        maxDate={maxDate}
                        maxRangeDays={maxRangeDays}
                        required
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ brandId, start, end, items = [] }) => {
    return useMemo(
        () => [
            textColumn({
                accessor: "username",
                Header: <Trans zh="用户名" en="Username" />,
                Cell: ({ value }) => <Text.Block>{value}</Text.Block>,
                Footer: () => <Trans zh="总计" en="Total" />,
                footerCellProps: {
                    textAlign: "center"
                },
                disableSortBy: true
            }),
            textColumn({
                id: "accountType",
                accessor: (row) => row,
                Header: <Trans zh="账户类型" en="Account Type" />,
                Cell: ({ value }) => (
                    <Text.Block>
                        <Trans zh="玩家" en="Player" />
                    </Text.Block>
                ),
                Footer: () => <Box />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "level",
                Header: <Trans zh="VIP 等级" en="VIP Level" />,
                Cell: ({ value }) => <Text.Block>VIP {value}</Text.Block>,
                Footer: () => <Box />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "depositAmount",
                sumColumn: "depositAmount",
                Header: <Trans zh="存款金额" en="Deposit Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "withdrawalAmount",
                sumColumn: "withdrawalAmount",
                Header: <Trans zh="提款金额" en="Withdrawal Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "bonusAmount",
                sumColumn: "bonusAmount",
                Header: <Trans zh="红利金额" en="Bonus Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "adjustmentAmount",
                sumColumn: "adjustmentAmount",
                Header: <Trans zh="调账" en="Adjustment Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "betAmount",
                sumColumn: "betAmount",
                Header: <Trans zh="投注金额" en="Bet Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "validBetAmount",
                sumColumn: "validBetAmount",
                Header: <Trans zh="有效投注" en="Valid Bet Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "winLossAmount",
                sumColumn: "winLossAmount",
                Header: <Trans zh="输赢金额" en="Win Loss Amount" />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "netProfit",
                sumColumn: "netProfit",
                Header: <Trans zh="净盈利" en="Net Profit" />,
                disableSortBy: true
            })
        ],
        [JSON.stringify(items)]
    )
}
