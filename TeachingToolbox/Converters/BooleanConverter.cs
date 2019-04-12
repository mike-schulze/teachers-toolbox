using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace TeachingToolbox.Converters
{
//==============================================================================
public class BooleanConverter<T> : IValueConverter
{

//------------------------------------------------------------------------------
// PUBLIC METHODS
//------------------------------------------------------------------------------

    public
    //--------------------------------------------------------------------------
    BooleanConverter
    //--------------------------------------------------------------------------
        (
        T aTrueValue,
        T aFalseValue
        )
    {
        True = aTrueValue;
        False = aFalseValue;
    }

    public virtual object
    //--------------------------------------------------------------------------
    Convert
    //--------------------------------------------------------------------------
        (
        object aValue, 
        Type aTargetType, 
        object aParameter, 
        CultureInfo aCulture
        )
    {
        return aValue is bool && ((bool)aValue) ? True : False;
    }

    public virtual object
    //--------------------------------------------------------------------------
    ConvertBack
    //--------------------------------------------------------------------------
        (
        object aValue, 
        Type aTargetType, 
        object aParameter, 
        CultureInfo aCulture
        )
    {
        return 
            (
            aValue is T && 
            EqualityComparer<T>.Default.Equals( (T) aValue, True )
            );
    }


//------------------------------------------------------------------------------
// PUBLIC PROPERTIES
//------------------------------------------------------------------------------

    public T True { get; set; }
    public T False { get; set; }

}

//==============================================================================
public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
{

//------------------------------------------------------------------------------
// PUBLIC METHODS
//------------------------------------------------------------------------------

    public
    //--------------------------------------------------------------------------
    BooleanToVisibilityConverter
    //--------------------------------------------------------------------------
        (
        ) 
    :
        base( Visibility.Visible, Visibility.Collapsed )
    { 
    }
}


//==============================================================================
public sealed class InverseBooleanConverter : BooleanConverter<bool>
{

    //------------------------------------------------------------------------------
    // PUBLIC METHODS
    //------------------------------------------------------------------------------

    public
    //--------------------------------------------------------------------------
    InverseBooleanConverter
        //--------------------------------------------------------------------------
        (
        )
        :
            base( false, true )
    {
    }
}

//==============================================================================
public class MultiBooleanToVisibilityConverter : IMultiValueConverter
{

//------------------------------------------------------------------------------
// PUBLIC METHODS
//------------------------------------------------------------------------------

    public object
    //--------------------------------------------------------------------------
    Convert
    //--------------------------------------------------------------------------
        (
        object[] aValues,
        Type aTargetType,
        object aParameter,
        System.Globalization.CultureInfo aCulture )
    {
        bool theIsVisible = true;
        foreach( object theValue in aValues )
        {
            if( theValue is bool )
                theIsVisible = theIsVisible && (bool) theValue;
        }

        if( theIsVisible )
        {
            return System.Windows.Visibility.Visible;
        }
        else
        {
            return System.Windows.Visibility.Collapsed;
        }
    }

    public object[]
    //--------------------------------------------------------------------------
    ConvertBack
    //--------------------------------------------------------------------------
        (
        object aValue,
        Type[] aTargetTypes,
        object aParameter,
        System.Globalization.CultureInfo aCulture
        )
    {
        throw new NotImplementedException();
    }
}

[ValueConversion(typeof(bool), typeof(GridLength))]
//==============================================================================
/// <summary>
/// Can be used to set a row's height or column's width to either * or 0
/// This allows us to have * rows/columns that can be hidden,
/// (Just collapsing the contents of a * row/column will still take up space)
/// </summary>
public class BoolToGridLengthConverter : IValueConverter
{

//------------------------------------------------------------------------------
// PUBLIC METHODS
//------------------------------------------------------------------------------

    public object
    //--------------------------------------------------------------------------
    Convert
    //--------------------------------------------------------------------------
        (
        object aValue, 
        Type aTargetType, 
        object aParameter, 
        CultureInfo aCulture
        )
    {
        if( (bool) aValue == true )
        {
            return new GridLength( 1, GridUnitType.Star );
        }
        else
        {
            return new GridLength( 0 );
        }
    }

    public object
    //--------------------------------------------------------------------------
    ConvertBack
    //--------------------------------------------------------------------------
        (
        object aValue,
        Type aTargetType,
        object aParameter,
        CultureInfo aCulture
        )
    {
        throw new NotImplementedException();
    }
}

//==============================================================================
/// Compares the value and parameter string values and converts to a bool
/// Note: this is used for RadioButton binding.  This should be removed and replaced with an enum
/// See http://stackoverflow.com/questions/397556/how-to-bind-radiobuttons-to-an-enum (the answer AND first comment for the unset value)
public class BooleanToStringConverter : IValueConverter
{
    public object
    //--------------------------------------------------------------------------
    Convert
    //--------------------------------------------------------------------------
        ( 
        object value, 
        Type targetType, 
        object parameter,
        System.Globalization.CultureInfo culture 
        )
    {
        if( System.Convert.ToString( value ).Equals( System.Convert.ToString( parameter ) ) )
        {
            return true;
        }
        return false;
    }

    public object 
    //--------------------------------------------------------------------------
    ConvertBack
    //--------------------------------------------------------------------------
        ( 
        object value, 
        Type targetType, 
        object parameter, 
        System.Globalization.CultureInfo culture 
        )
    {
        // Only bind this back if it is true (selected)
        if( System.Convert.ToBoolean( value ) )
        {
            return parameter;
        }
        return DependencyProperty.UnsetValue;
    }
}

//==============================================================================
/// <summary>
/// Converter that checks whether it's the correct enum.
/// </summary>
public class EnumMatchToBooleanConverter : IValueConverter
{
    //--------------------------------------------------------------------------
    public object
    Convert
        (
        object aValue,
        Type aTargetType,
        object aParameter,
        CultureInfo aCulture
        )
    {
        if( aValue == null || aParameter == null )
        {
            return false;
        }

        return aValue.ToString() == aParameter.ToString();
    }

    //--------------------------------------------------------------------------
    public object
    ConvertBack
        (
        object aValue,
        Type aTargetType,
        object aParameter,
        CultureInfo aCulture
        )
    {
        if( aValue == null || aParameter == null )
        {
            return DependencyProperty.UnsetValue;
        }

        if( aValue is bool && (bool)aValue )
        {
            return Enum.Parse( aTargetType, aParameter.ToString() );
        }

        return DependencyProperty.UnsetValue;
    }
}  

}
